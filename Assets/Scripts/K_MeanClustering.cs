using UnityEditor;
using UnityEditor.Scripting.Python;

public class MenuItem_KMeanClustering_Class
{
   [MenuItem("Python Scripts/KMeanClustering")]
   public static void KMeanClustering()
   {
       PythonRunner.RunFile("Assets/Scripts/K_MeanClustering.py");
   }
};
