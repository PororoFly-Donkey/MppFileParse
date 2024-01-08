using net.sf.mpxj.reader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = net.sf.mpxj.Task;
using Newtonsoft.Json;
using System.IO;

using java.lang;
using net.sf.mpxj;

namespace MppFileParse
{
    internal class Program
    {
        static int MapTaskTypeToInt(string taskType)
        {
            switch (taskType)
            {
                case "FIXED_UNITS":
                    return 0;
                case "FIXED_DURATION":
                    return 1;
                case "FIXED_WORK":
                    return 2;
                case "FIXED_DURATION_AND_UNITS":
                    return 3;
                // Add more cases as needed
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        static void Main(string[] args)
        {
            ArrayList mppList = new ArrayList();

            foreach (string folder in Directory.EnumerateDirectories(args[0]))
            {
                foreach (string file in Directory.EnumerateFiles(folder))
                {
                    var project = new UniversalProjectReader().read(file);
                    foreach (Task task in project.getTasks())
                    {
                        DateTime dt = DateTime.Now;
                        string batch_name = dt.ToString("yyyyMMddHHmmssfff");

                        System.Threading.Thread.Sleep(1);


                        string project_name = folder.Split('\\')[folder.Split('\\').Length - 1];

                        string task_name = task.getName();
                        int task_type = MapTaskTypeToInt(task.getType().toString());
                        
                        string duration_days = System.Double.Parse(task.getDuration().getDuration().ToString()).ToString("F2");
                        string start_date = task.getStart().toLocalDate().toString();
                        string end_date = task.getFinish().toLocalDate().toString();

                        string predecessor = "";
                        java.util.List predecessors = task.getPredecessors();
                        if (!predecessors.isEmpty())
                        {
                            System.Text.RegularExpressions.Regex rgx = new System.Text.RegularExpressions.Regex(@"(?i)->\s*\[Task\s*id=(\d+)");

                            foreach (System.Text.RegularExpressions.Match m in rgx.Matches(predecessors.ToString()))
                                predecessor = m.Groups[1].Value.ToString();

                        }

                        string level = task.getOutlineLevel().toString();
                        string notes = task.getNotes().ToString();

                        mppList.Add(new
                        {
                            batch_name = batch_name,
                            project_name = project_name,
                            task_name = task_name,
                            task_type = task_type,
                            duration_days = duration_days,
                            start_date = start_date,
                            end_date = end_date,
                            predecessor = predecessor,
                            level = level,
                            notes = notes,
                        });

                    }
                }
            }
            string json = JsonConvert.SerializeObject(mppList);
            System.Console.Write(json);
        }
    }
}
