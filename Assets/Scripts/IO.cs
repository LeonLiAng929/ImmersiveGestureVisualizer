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
                    gestures.Add(temp);
                }
            }
        }
        return gestures;
    }
    /*
    private static void ReadXML()
    {
        for (int i = 1; i < 31; i++)
        {
            string fileName = Path.Combine(Application.dataPath + "/Resources/"+i.ToString()+"/angry like a bear-1.xml");
        
            DataTable newTable = new DataTable();
            newTable.ReadXmlSchema(fileName);
            newTable.ReadXml(fileName);

            // Print out values in the table.
            PrintValues(newTable, "New table " + i.ToString());
        }
    }

    private static DataTable CreateTestTable(string tableName)
    {
        // Create a test DataTable with two columns and a few rows.
        DataTable table = new DataTable(tableName);
        DataColumn column = new DataColumn("id", typeof(System.Int32));
        column.AutoIncrement = true;
        table.Columns.Add(column);

        column = new DataColumn("item", typeof(System.String));
        table.Columns.Add(column);

        // Add ten rows.
        DataRow row;
        for (int i = 0; i <= 9; i++)
        {
            row = table.NewRow();
            row["item"] = "item " + i;
            table.Rows.Add(row);
        }

        table.AcceptChanges();
        return table;
    }

    private static void PrintValues(DataTable table, string label)
    {
        Debug.Log(label);
        foreach (DataRow row in table.Rows)
        {
            foreach (DataColumn column in table.Columns)
            {
                Debug.Log(row[column]);
            }
            Debug.Log("");
        }
    }
    */
}
