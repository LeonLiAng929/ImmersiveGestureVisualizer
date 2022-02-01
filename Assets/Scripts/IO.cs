using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml; 
using System.IO;
using System.Xml.Linq; //Needed for XDocument
using System.Data;
using System.Linq;

public class IO
{
    public XDocument xmlDoc; //create Xdocument. Will be used later to read XML file 
    public IEnumerable<XElement> items; //Create an Ienumerable list. Will be used to store XML Items.

    public List<Gesture> LoadXML(string fileName)
    {
        List<Gesture> gestures = new List<Gesture>();
        for (int i = 1; i < 31; i++)
        {
            string sourceDirectory = Application.dataPath + "/Resources/" + i.ToString() + "/";

            var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*.xml");

            foreach (string currentFile in txtFiles)
            {
                string name = currentFile.Substring(sourceDirectory.Length);

                if (name.Contains(fileName))
                {
                    name = name.Substring(0, name.Length - 4);
                    TextAsset xmlTextAsset = Resources.Load<TextAsset>(i.ToString() + "/" + name);

                    XmlDocument gestureDataXml = new XmlDocument();
                    gestureDataXml.LoadXml(xmlTextAsset.text);


                    XmlNodeList postureLi = gestureDataXml.SelectNodes("/BodyGesture/BodyPosture");

                    Gesture temp = new Gesture();

                    XmlNode gesture_node = gestureDataXml.SelectSingleNode("BodyGesture");
                    temp.gestureType = gesture_node.Attributes["Name"].Value;
                    temp.id = i;
                    temp.trial = name[name.IndexOf('-') + 1];
                    temp.num_of_poses = int.Parse(gesture_node.Attributes["Postures"].Value);
                    foreach (XmlNode x in postureLi)
                    {
                        Pose temp_p = new Pose();
                        temp_p.timestamp = double.Parse(x.Attributes["Time"].Value);
                        temp_p.num_of_joints = int.Parse(x.Attributes["Joints"].Value);
                        XmlNodeList jointLi = x.SelectNodes("Joint");
                        foreach (XmlNode y in jointLi)
                        {
                            Joint temp_q = new Joint();
                            temp_q.jointType = y.Attributes["Type"].Value;
                            temp_q.x = float.Parse(y.Attributes["X"].Value);
                            temp_q.y = float.Parse(y.Attributes["Y"].Value);
                            temp_q.z = float.Parse(y.Attributes["Z"].Value);
                            temp_p.joints.Add(temp_q);
                        }
                        temp.poses.Add(temp_p);
                    }
                    temp.SetBoundingBox();
                    temp.SetCentroid();
                    temp.NormalizeTimestamp();
                    gestures.Add(temp);
                }
            }
        }
        return gestures;
    }

    /// <summary>
    /// Parse gesture file to list of handposes.
    /// </summary>
    /// <param name="text">csv text</param>
    /// <returns></returns>
    public Gesture LoadHandGesture(string fileName)
    {
        TextAsset sampleHandGesture = Resources.Load<TextAsset>("SampleHandGes");
        string text = sampleHandGesture.text;
        Gesture handGes = new Gesture();
        List<string> lines = text.Split('\n').ToList();
        handGes.num_of_poses = lines.Count;
        handGes.trial = '1'; // hard-coded for now, change later.
        handGes.id = 1;
        handGes.gestureType = "HandGesture";
        List<string> legends = lines[0].Split(',').ToList();
        legends.RemoveAt(0);
        lines.RemoveAt(0);
        lines.RemoveAt(lines.Count - 1);
        foreach (string row in lines)
        {
            List<string> points = row.Split(',').ToList();
            Pose handPose = new Pose();
            handPose.timestamp = int.Parse(points[0]);
            points.RemoveAt(0);
            if (points.Count != 63)
                continue;
            handPose.num_of_joints = points.Count / 3;
            for (int i = 0; i < points.Count; i += 3)
            {
                Joint joint = new Joint();
                joint.jointType = legends[i].Substring(0, 8);
                joint.x = float.Parse(points[i].Trim());
                joint.y = float.Parse(points[i + 1].Trim());
                joint.z = float.Parse(points[i + 2].Trim());
                handPose.joints.Add(joint);
            }
            handGes.poses.Add(handPose);
        }
        handGes.SetBoundingBox();
        handGes.SetCentroid();
        handGes.NormalizeTimestamp();

        return handGes;
    }

}
