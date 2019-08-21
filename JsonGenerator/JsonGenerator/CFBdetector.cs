using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonGenerator
{
    class CFBdetector
    {
        public List<Cockpit_Component_Json> componentsList = new List<Cockpit_Component_Json>();
        public List<FeedbackGroup> feedback_groupList = new List<FeedbackGroup>();
        public List<Container_Box_Json> boxesList = new List<Container_Box_Json>();

        public CFBdetector(String path, String sequence, String sequenceNewName)
        {
            //get components, feedback_group and boxes
            String firstJsonPath = String.Format(path + "savedObjects_fixed.json");
            JsonSaveObject data = null;
            using (StreamReader reader = new StreamReader(new FileStream(firstJsonPath, FileMode.Open)))
            {
                try
                {
                    String line = null;
                    StringBuilder fullFile = new StringBuilder();
                    while ((line = reader.ReadLine()) != null)
                    {
                        fullFile.Append(line).Append("\n\r");
                    }
                    data = JsonConvert.DeserializeObject<JsonSaveObject>(fullFile.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.GetBaseException().ToString());
                    Console.ReadLine();
                }
            }

            //filtro i boxes
            foreach (Container_Box_Json box in data.boxes)
                if (box.sequence.Equals(sequence))
                {
                    //aggiorno nome sequenza
                    box.sequence = sequenceNewName;
                    this.boxesList.Add(box);
                }

            //filtro i feedback_groups
            List<String> oggetti = new List<string>();
            foreach (FeedbackGroup feedback in data.feedback_group)
                if (feedback.sequence.Equals(sequence))
                {
                    //aggiorno nome sequenza
                    feedback.sequence = sequenceNewName;
                    this.feedback_groupList.Add(feedback);
                    foreach(CockpitFeedback oggettoIndicato in feedback.objectIndicated)
                        oggetti.Add(oggettoIndicato.name.ToString());
                }

            //filtro i components
            foreach (Cockpit_Component_Json component in data.components)
                foreach (String oggetto in oggetti)
                    if (component.nameObject.Equals(oggetto))
                    {
                        this.componentsList.Add(component);
                        break;
                    }
        }
    }
}
