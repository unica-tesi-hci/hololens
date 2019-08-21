using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using System.Text;

namespace JsonGenerator
{
    class TaskListCreator
    {
        public static List<Task> Generate(String path, String xmlPath, String fileName, Boolean releasePatternBlocker, Boolean cognitiveNeeded)
        {
            //create tasklist
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(File.ReadAllText(xmlPath));
            XmlNodeList datas = doc.SelectNodes("hamsters").Item(0).SelectNodes("datas").Item(0).ChildNodes;
            XmlNode root = doc.SelectNodes("hamsters").Item(0).SelectNodes("nodes").Item(0).SelectSingleNode("task").SelectSingleNode("operator");
            XmlNodeList branch = getProperBranch(root, releasePatternBlocker, cognitiveNeeded);

            int firstID = 0;
            int noLastID = 0;
            int lastID = 0;

            //stabilisco ordine
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas.Item(i).Attributes.GetNamedItem("name").Value.Contains("Suggestion"))
                    firstID = Convert.ToInt32(datas.Item(i).FirstChild.Attributes.GetNamedItem("sourceid").Value);
                else if (datas.Item(i).Attributes.GetNamedItem("name").Value.Contains("Confirmation"))
                {
                    noLastID = Convert.ToInt32(datas.Item(i).FirstChild.Attributes.GetNamedItem("sourceid").Value);
                    lastID = Convert.ToInt32(datas.Item(i).LastChild.Attributes.GetNamedItem("sourceid").Value);
                }
            }

            //creo lista task
            List<XmlNode> taskList = getTasks(branch, cognitiveNeeded);

            //creo task necessari mancanti
            fixTaskList(taskList);

            //riordino
            List<KeyValuePair<int, int>> sortingTool = new List<KeyValuePair<int, int>>();
            int counter = 0;
            foreach (XmlNode node in taskList)
            {
                KeyValuePair<int, int> pair = new KeyValuePair<int, int>(counter, Convert.ToInt32(node.Attributes.GetNamedItem("x").Value.ToString()));
                sortingTool.Add(pair);
                counter++;
            }
            sortingTool.Sort(delegate (KeyValuePair<int, int> pair1,
                                KeyValuePair<int, int> pair2)
            {
                return pair1.Value.CompareTo(pair2.Value);
            });
            List<XmlNode> toSave = new List<XmlNode>();
            foreach (KeyValuePair<int, int> p in sortingTool)
                toSave.Add(taskList[p.Key]);

            List<Task> toReturn = toJSON(toSave, fileName);

            return toReturn;
        }

        private static XmlNodeList getProperBranch(XmlNode root, Boolean releasePatternBlocker, Boolean cognitiveNeeded)
        {
            XmlNodeList firstBranch = root.SelectNodes("task");

            int i = 0;
            foreach (XmlNode n in firstBranch)
            {
                //flag
                if (cognitiveNeeded)
                {
                    if (n.Attributes.GetNamedItem("type").Value.Equals("cognitive", StringComparison.CurrentCultureIgnoreCase) && n.Attributes.GetNamedItem("name").Value.Contains("Wait"))
                        return firstBranch;
                }

                if (!cognitiveNeeded)
                {
                    if ((n.Attributes.GetNamedItem("type").Value.Equals("input", StringComparison.CurrentCultureIgnoreCase) && (!n.Attributes.GetNamedItem("name").Value.Contains("Release") || !releasePatternBlocker))
                       || n.Attributes.GetNamedItem("type").Value.Equals("user", StringComparison.CurrentCultureIgnoreCase))
                        return firstBranch;
                }

                if (n.Attributes.GetNamedItem("type").Value.Equals("abstract", StringComparison.CurrentCultureIgnoreCase))
                {
                    XmlNodeList tmp = getProperBranch(n, releasePatternBlocker, cognitiveNeeded);
                    if (tmp != null)
                        if (tmp.Count > 0)
                            return tmp;
                }
                
            }

            //XmlNode secondBranch = root.SelectSingleNode("operator");
            XmlNodeList secondBranch = root.SelectNodes("operator");
            if (secondBranch == null)
                return null;
            foreach (XmlNode branch in secondBranch)
            {
                XmlNodeList toRet = getProperBranch(branch, releasePatternBlocker, cognitiveNeeded);
                if (toRet != null)
                    return toRet;
            }
            return null;
        }

        private static void addAttribute(XmlNode item, String attributeName)
        {
            XmlAttribute attribute = item.OwnerDocument.CreateAttribute(attributeName);
            attribute.Value = "";
            item.Attributes.Append(attribute);
        }

        private static void deleteAttribute(XmlNode item, String attributeName)
        {
            XmlAttribute attribute = null;
            foreach (XmlAttribute attri in item.Attributes)
            {
                if (attri.Name.Equals(attributeName))
                    attribute = attri;
            }

            item.Attributes.Remove(attribute);
        }

        private static List<XmlNode> getTasks(XmlNodeList nodes, Boolean cognitiveNeeded)
        {
            List<XmlNode> taskList = new List<XmlNode>();

            foreach (XmlNode node in nodes)
            {
                //se ho trovato un task foglia, lo prendo
                if (node.OuterXml.Split()[0].Contains("task") && node.ChildNodes.Count == 0)
                {
                    deleteAttribute(node, "help");
                    deleteAttribute(node, "copy");
                    deleteAttribute(node, "folded");
                    deleteAttribute(node, "critical");
                    addAttribute(node, "sequence");

                    String str = node.Attributes.GetNamedItem("type").Value.ToString();
                    if ((str.Equals("input") && !cognitiveNeeded)|| str.Equals("output") || (str.Equals("user") && !cognitiveNeeded) || (str.Equals("cognitive") && cognitiveNeeded) || (str.Equals("sight") && node.Attributes.GetNamedItem("name").Value.Contains("Good")))
                        taskList.Add(node);
                }
                //altrimenti se ha figli
                else if (node.OuterXml.Split()[0].Contains("task") && node.ChildNodes.Count > 0)
                {
                    foreach (XmlNode child in node.ChildNodes)
                        if (child.OuterXml.Split()[0].Contains("operator") && (child.Attributes.GetNamedItem("type").Value.ToString().Equals("enable") || child.Attributes.GetNamedItem("type").Value.ToString().Equals("concurrent")))
                        {
                            //task figli dell'operator
                            List<XmlNode> t = getTasks(child.ChildNodes, cognitiveNeeded);
                            foreach (XmlNode c in t)
                                taskList.Add(c);
                        }
                }
                else if (node.OuterXml.Split()[0].Contains("operator") && (node.Attributes.GetNamedItem("type").Value.ToString().Equals("enable") || node.Attributes.GetNamedItem("type").Value.ToString().Equals("concurrent")))
                {
                    //task figli dell'operator
                    List<XmlNode> t = getTasks(node.ChildNodes, cognitiveNeeded);
                    foreach (XmlNode c in t)
                        taskList.Add(c);
                }
            }

            return taskList;
        }

        private static void fixTaskList(List<XmlNode> taskList)
        {
            bool displayInfo = false;
            bool displayConfirmation = false;
            bool seeConfirmation = false;

            String disInfo = "Display 3D info";
            String disConf = "Display 3D confirmation";
            String seeConf = "See the confirmation: \"Good\"";

            foreach (XmlNode t in taskList)
            {
                if (t.Attributes.GetNamedItem("name").Value.Equals(disInfo))
                    displayInfo = true;
                else if (t.Attributes.GetNamedItem("name").Value.Equals(disConf))
                    displayConfirmation = true;
                else if (t.Attributes.GetNamedItem("name").Value.Equals(seeConf))
                    seeConfirmation = true;
            }

            if (!displayInfo)
            {
                XmlNode dis = taskList[0].CloneNode(true); ;
                dis.Attributes.GetNamedItem("name").Value = disInfo;
                dis.Attributes.GetNamedItem("x").Value = "0";
                dis.Attributes.GetNamedItem("duration").Value = "false";
                dis.Attributes.GetNamedItem("nb_iteration").Value = "0";
                dis.Attributes.GetNamedItem("type").Value = "output";

                taskList.Insert(0, dis);
            }

            if (!seeConfirmation)
            {
                XmlNode see = taskList[taskList.Count - 1].CloneNode(true); ;
                see.Attributes.GetNamedItem("name").Value = seeConf;
                if(Convert.ToInt32(see.Attributes.GetNamedItem("x").Value) > 0)
                    see.Attributes.GetNamedItem("x").Value = (Convert.ToInt32(see.Attributes.GetNamedItem("x").Value) + 5).ToString();
                else
                    see.Attributes.GetNamedItem("x").Value = (Convert.ToInt32(see.Attributes.GetNamedItem("x").Value)*(-1) + 5).ToString();
                see.Attributes.GetNamedItem("duration").Value = "false";
                see.Attributes.GetNamedItem("nb_iteration").Value = "0";
                see.Attributes.GetNamedItem("type").Value = "sight";

                taskList.Add(see);
            }

            if (!displayConfirmation)
            {
                XmlNode dis = taskList[taskList.Count - 1].CloneNode(true);
                dis.Attributes.GetNamedItem("name").Value = disConf;
                if (Convert.ToInt32(dis.Attributes.GetNamedItem("x").Value) > 0)
                    dis.Attributes.GetNamedItem("x").Value = (Convert.ToInt32(dis.Attributes.GetNamedItem("x").Value) - 2).ToString();
                else
                    dis.Attributes.GetNamedItem("x").Value = (Convert.ToInt32(dis.Attributes.GetNamedItem("x").Value) * (-1) -2).ToString();
                dis.Attributes.GetNamedItem("duration").Value = "false";
                dis.Attributes.GetNamedItem("nb_iteration").Value = "0";
                dis.Attributes.GetNamedItem("type").Value = "output";

                taskList.Insert(taskList.Count - 2, dis);
            }

        }

        private static List<Task> toJSON(List<XmlNode> taskList, String outputFile)
        {
            //scrivo il file
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml("<infos></infos>");

            String name = outputFile.Replace(" ", "_");
            name = name.ToUpper();

            String taskSection = "<subroutine>";

            foreach (XmlNode node in taskList)
            {
                XmlNode newEmp = xdoc.ImportNode(node, true);
                taskSection = taskSection + newEmp.OuterXml;
            }
            
            XmlDocument doc2 = new XmlDocument();
            doc2.LoadXml(taskSection + " </subroutine>");
            String json = JsonConvert.SerializeObject(doc2, Newtonsoft.Json.Formatting.Indented);
            json = json.Replace("  \"subroutine\": {\r\n", " \r\n");
            json = json.Replace("\"task\":", "\"tasks\":");
            json = json.Replace("@", "");
            json = json.Substring(0, json.LastIndexOf('}'));

            int ind = json.LastIndexOf("  }");
            json = json.Remove(ind, 2);

            ind = json.LastIndexOf("}");
            json = json.Remove(ind, 2);
            json = json.Replace("{\r\n \r\n    \"tasks\": [", "[");

            List<Task> tList = JsonConvert.DeserializeObject<List<Task>>(json);
            return tList;
        }
    }
}
