using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;

namespace JsonGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            String path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).Replace("JsonGenerator\\bin\\Debug", "");
            
            //usare il metodo Start per gli hmst che si vuole convertire
            //    sotto sono presenti esempi di conversioni fatte finora

            //Start(path, "releaseBrake", "BRAKE_RELEASE", "BRAKE_RELEASE", true, false);
            //Start(path, "Check 100 knots", "CHECK_100_KNOTS", "VELOCITY2", true, false);
            //Start(path, "Gear up", "GEAR_UP", "GEAR_OFF", true, false);
            //Start(path, "FLAPS 0", "FLAPS_0", "FLAPS2", true, false);
            //Start(path, "FLAPS 1", "FLAPS_1", "FLAPS", true, false);
            //Start(path, "Reach 80 knots N.P.", "CHECK_80_KNOTS", "VELOCITY", true, true);
            //Start(path, "Release THR Levers", "RELEASE_THR_LEVERS", "THROTTLE_FULL", false, false);
            //Start(path, "Directional control", "ACCELERATION", "ACCELERATION", true, false);
            //Start(path, "Gain altitude", "GAIN ALTITUDE", "TAKEOFF", true, false);
        }

        /**
         *  dir : folder of hmst/xml
         *  inputFile: name of hmst
         *  outputFile: directory of destination json file
         *  sequence: sequence name (must be the same of the original json savedObject file)
         *  releasePatterBlocker: if true, it skips branches with release input type
         **/
        public static void Start(String dir, String inputFile, String outputFile, String sequence, Boolean releasePatternBlocker, Boolean cognitiveNeeded)
        {
            //rilevo components, boxes e feedbackgroup
            CFBdetector CFBdet = new CFBdetector(dir, sequence, outputFile);

            //generlo la lista dei task
            JsonSaveObject json = new JsonSaveObject();
            json.name = outputFile;
            String hmstDir = String.Format(dir + "hmst/" + inputFile + ".hmst");
            String jsonDir = String.Format(dir + inputFile + ".json");
            List<Task> tasks = TaskListCreator.Generate(dir, hmstDir, inputFile, releasePatternBlocker, cognitiveNeeded);

            //inizializzo i campi del json
            json.tasks = tasks;

            //nel caso in cui ci siano chiedo se selezionare tutti i componenti/feedback_group
            if (CFBdet.componentsList.Count > 1)
            {
                Console.WriteLine("Ci sono {0} componenti", CFBdet.componentsList.Count);
                
                Cockpit_Component_Json[] componentsListCopy = new Cockpit_Component_Json[CFBdet.componentsList.Count];
                CFBdet.componentsList.CopyTo(componentsListCopy);
                for (int idx = 0; idx < CFBdet.componentsList.Count; idx++)
                {
                    Cockpit_Component_Json comp = CFBdet.componentsList[idx];

                    String risposta = "";
                    do
                    {
                        Console.WriteLine("Vuoi selezionare il seguente componente? S/N");
                        Console.WriteLine("--- Nome = " + comp.nameObject);
                        Console.WriteLine("--- Parametro = " + comp.parameter[0]);
                        Console.WriteLine("--- Path parametro = " + comp.param_path[0]);

                        risposta = Console.ReadLine();
                        if (risposta.Equals("N") || risposta.Equals("n"))
                        {
                            CFBdet.componentsList.Remove(comp);
                            CFBdet.feedback_groupList[0].objectIndicated.Remove(CFBdet.feedback_groupList[0].objectIndicated[idx]);
                            idx--;
                        }
                    }
                    while (!risposta.Equals("S") && !risposta.Equals("s") && !risposta.Equals("N") && !risposta.Equals("n"));
                }
            }

            json.components = CFBdet.componentsList;
            json.feedback_group = CFBdet.feedback_groupList;
            json.boxes = CFBdet.boxesList;

            //salvo il file
            String jsonText = JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(String.Format(dir + "result\\" + outputFile + ".json"), jsonText);
        }
    }
}
