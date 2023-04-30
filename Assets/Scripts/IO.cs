using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml; 
using System.IO;
using System.Xml.Linq; //Needed for XDocument
using System.Data;
using System.Linq;
using System;

public class IO
{
    public class Record
    {
        /// <summary>
        /// Each record contains summarised info about a cluster.
        /// </summary>
        public float consensus;
        public float matchedPairs = 0;
        public float totalPairs = 0;
        public List<Tuple<string, int>> referents = new List<Tuple<string, int>>();
    }

    public XDocument xmlDoc; //create Xdocument. Will be used later to read XML file 
    public IEnumerable<XElement> items; //Create an Ienumerable list. Will be used to store XML Items.

    public List<Gesture> LoadXML(string fileName, bool hideLabel=false)
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
                    if (hideLabel)
                    {
                        temp.gestureType = "Unknown";
                    }
                    else
                    {
                        temp.gestureType = gesture_node.Attributes["Name"].Value;
                    }
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
  

    public void WriteFeatureCount()
    {
        string filename = Application.dataPath + "/UserStudy/FeatureCount.csv";
        int[] count = UserStudy.instance.featureCount;
        if (!File.Exists(filename)) 
        {
            TextWriter tww = new StreamWriter(filename, false);

            string header = "";
            
            for(int i =0;i<count.Length;i++)
            {
                header += "," + ((Actions)i).ToString();
            }

            header = "Username" + header;
            tww.WriteLine(header);
            tww.Close();
        }
        TextWriter tw = new StreamWriter(filename, true);
        string content = "";//GestureAnalyser.instance.username+","+GestureAnalyser.instance.referent;
        for (int i = 0; i < count.Length; i++)
        {
            content += "," + count[i].ToString();
        }
        tw.WriteLine(content);
        tw.Close();
    }

    public void WriteUserResult()
    {
        string filename = Application.dataPath + "/UserStudy/UserRefinedCluster.csv";
        if (!File.Exists(filename))
        {
            TextWriter tww = new StreamWriter(filename, false);

            string header = "Username,OverallAccuracy";

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
        //content += GestureAnalyser.instance.referent;
        
        List<GestureGameObject> gestureGameObjects = gestureVisualizer.gestureGameObjs;
        Dictionary<string, int> clusteringResultDic = new Dictionary<string, int>();
        foreach(GestureGameObject gGO in gestureGameObjects)
        {
            string key = gGO.gesture.id.ToString() + "-" + gGO.gesture.trial.ToString();
            int cluster = gGO.gesture.cluster;
            clusteringResultDic.Add(key, cluster);
        }

        float correctPairs = 0;
        float totalPairs = 0;
        //string temp = "";
        var output = CollectClusteringOutputs();
        foreach (Record r in output)
        {
            correctPairs += r.matchedPairs;
            totalPairs += r.totalPairs;
            /*string cell = ",";
            cell += r.consensus.ToString();
            string tuples = "";
            foreach (Tuple<string, int> t in r.referents)
            {
                tuples += t.ToString() + ",";
            }
            cell += "," + string.Format("{0}", "\"" + tuples + "\"");
            temp += cell;*/
        }

        content += (correctPairs / totalPairs).ToString();
        //content += temp;

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

    public List<string> GetTrainingData()
    {
        string filename = Application.dataPath + "/UserStudy/TrainingClusters.csv";
        TextReader tr = new StreamReader(filename, false);
        tr.ReadLine();
        List<string> values = tr.ReadLine().Split(new string[] { "," }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
        values.RemoveAt(0);
        tr.Close();
        return values;
    }

    public List<Record> CollectClusteringOutputs()
    {
        List<Record> records = new List<Record>();
        foreach (KeyValuePair<int, Cluster> pair in GestureAnalyser.instance.GetClusters())
        {
            Record r = WithinClusterDataProcessing(pair.Value);
            records.Add(r);
        }
        var output = records.OrderByDescending(x => x.consensus).ToList();
        return output;
    }

    public Record WithinClusterDataProcessing(Cluster c)
    {
        List<Gesture> gestures = c.GetGestures();
        if (gestures.Count > 1)
        {
            Dictionary<string, int> count = new Dictionary<string, int>(); // count of gestures in each refent presented in this cluster
            int numOfGestures = c.GestureCount();
            float similarPairs = 0;
            for (int i = 0; i < numOfGestures - 1; i++)
            {
                string key = gestures[i].trial.ToString();
                if (count.ContainsKey(key))
                {
                    count[key] += 1;
                }
                else
                {
                    count.Add(key, 1);
                }
                for (int j = i + 1; j < numOfGestures; j++)
                {
                    if (gestures[i].trial == gestures[j].trial)
                    {
                        similarPairs += 1;
                    }
                }
            }
            string keyy = gestures[numOfGestures - 1].trial.ToString();
            if (count.ContainsKey(keyy))
            {
                count[keyy] += 1;
            }
            else
            {
                count.Add(keyy, 1);
            }
            float consensus = similarPairs / (0.5f * numOfGestures * (numOfGestures - 1));
            Record r = new Record();
            r.consensus = consensus;
            r.matchedPairs = similarPairs;
            r.totalPairs = numOfGestures * (numOfGestures - 1) / 2;
            /*List<Tuple<string, int>> referents = new List<Tuple<string, int>>();
            foreach (KeyValuePair<string, int> pair in count)
            {
                referents.Add(new Tuple<string, int>(pair.Key, pair.Value));
            }
            var result = referents.OrderByDescending(x => x.Item2).ToList();
            r.referents = result;*/
            return r;
        }
        else
        {
            Record r = new Record();
            r.consensus = -1;
            r.matchedPairs = 0;
            r.totalPairs = 0;
            var li = new List<Tuple<string, int>>();
            li.Add(new Tuple<string, int>(gestures[0].gestureType, 1));
            r.referents = li;
            return r;
        }
    }

}