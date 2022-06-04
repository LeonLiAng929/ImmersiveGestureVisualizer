using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml; 
using System.IO;
using System.Xml.Linq; //Needed for XDocument
using System.Data;
public class IO
{
    public XDocument xmlDoc; //create Xdocument. Will be used later to read XML file 
    public IEnumerable<XElement> items; //Create an Ienumerable list. Will be used to store XML Items.

    public List<Gesture> LoadXML(string fileName)
    {
       List<Gesture> gestures = new List<Gesture>();   
       for (int i = 1; i < 31; i++)
        {
            string sourceDirectory = Application.dataPath + "/Resources/" + i.ToString()+"/";

            var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*.xml");

            foreach (string currentFile in txtFiles)
            {
                string name = currentFile.Substring(sourceDirectory.Length);

                if (name.Contains(fileName))
                {
                    name = name.Substring(0, name.Length-4);
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
                    temp.TranslateToOrigin();
                    temp.NormalizeHeight();
                    gestures.Add(temp);
                }
            }
        }
        return gestures;
    }


    public List<Gesture> LoadAllXML()
    {
        List<Gesture> gestures = new List<Gesture>();
        for (int i = 1; i < 31; i++)
        {
            string sourceDirectory = Application.dataPath + "/Resources/" + i.ToString() + "/";

            var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*.xml");

            foreach (string currentFile in txtFiles)
            {
                string name = currentFile.Substring(sourceDirectory.Length);
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
                temp.TranslateToOrigin();
                temp.NormalizeHeight();
                gestures.Add(temp);
            }
        }
        return gestures;
    }

    public void WriteFeatureCount()
    {
        string filename = Application.dataPath + "/FeatureCount.csv";
        int[] count = UserStudy.instance.featureCount;
        if (!File.Exists(filename)) 
        {
            TextWriter tww = new StreamWriter(filename, false);

            string header = "";
            
            for(int i =0;i<count.Length;i++)
            {
                header += "," + ((Actions)i).ToString();
            }

            header = "Username,Referent" + header;
            tww.WriteLine(header);
            tww.Close();
        }
        TextWriter tw = new StreamWriter(filename, true);
        string content = GestureAnalyser.instance.username+","+GestureAnalyser.instance.referent;
        for (int i = 0; i < count.Length; i++)
        {
            content += "," + count[i].ToString();
        }
        tw.WriteLine(content);
        tw.Close();
    }

    public void WriteUserResult()
    {
        string filename = Application.dataPath + "/UserRefinedCluster.csv";
        if (!File.Exists(filename))
        {
            TextWriter tww = new StreamWriter(filename, false);

            string header = "Username,Referent";

            for (int i = 1; i < 31; i++)
            {
                for (int j = 1; j < 4; j++)
                {
                    header += "," + "P"+i.ToString() + "-T"+j.ToString();
                }
            }

            
            tww.WriteLine(header);
            tww.Close();
        }

        GestureVisualizer gestureVisualizer = GestureVisualizer.instance;
        string content = "";
        content += GestureAnalyser.instance.username;
        content += ",";
        content += GestureAnalyser.instance.referent;
        List<GestureGameObject> gestureGameObjects = gestureVisualizer.gestureGameObjs;
        Dictionary<string, int> clusteringResultDic = new Dictionary<string, int>();
        foreach(GestureGameObject gGO in gestureGameObjects)
        {
            string key = gGO.gesture.id.ToString() + "-" + gGO.gesture.trial.ToString();
            int cluster = gGO.gesture.cluster;
            clusteringResultDic.Add(key, cluster);
        }
        TextWriter tw = new StreamWriter(filename, true);
        
        for (int i = 1; i < 31; i++)
        {
            for(int j = 1; j < 4; j++)
            {
                string k = i.ToString() + "-" + j.ToString();
                if (clusteringResultDic.ContainsKey(k))
                    content += "," + clusteringResultDic[k].ToString();
                else
                {
                    content += ","; 
                }
            }
        }
        tw.WriteLine(content);
        tw.Close();
    }


}
