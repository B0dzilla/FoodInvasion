using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Text;
using System;
using System.Threading;

public class BundleCreate
{

    public static string projectpath;
    public static string binpath;

    private static bool isInit = false;
    static void Init()
    {
        if (isInit) return;

        projectpath = Application.dataPath.Replace("Assets", "");


        binpath = projectpath + "bin";
        if (!Directory.Exists(binpath))
        {
            Directory.CreateDirectory(binpath);
        }

        isInit = true;
    }

    [MenuItem("Build/Full build")]
    static void FullBuild()
    {
        Init();

        BuildBundle();

        Build();
    }
    [MenuItem("Build/Build + Zip")]
    static void BuildZip()
    {
        Init();

        string platform = EditorUserBuildSettings.activeBuildTarget.ToString();

        if (!Directory.Exists(binpath + "/" + platform))
        {
            Directory.CreateDirectory(binpath + "/" + platform);
        }
        string outputPath = string.Empty;

        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                outputPath = binpath + "/" + platform + "/" + PlayerSettings.productName + ".apk";
                break;
            case BuildTarget.iOS:
                {

                    outputPath = binpath + "/" + platform + "/" + PlayerSettings.productName + "_xcode";

                    if (!Directory.Exists(outputPath))
                    {
                        Directory.CreateDirectory(outputPath);
                    }
                }
                break;

        }


        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputPath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);

        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            //C:\Users\DELAKEY>powershell "D:\Source\The-moon\Assets\Editor\zipper.vbs" "D:\Source\The-moon\Assets\Editor\111" "D:\Source\The-moon\Assets\Editor\111.zip"
            XcodeToZip();
        }
    }
    [MenuItem("Build/Build")]
    static void Build()
    {
        Init();

        string platform = EditorUserBuildSettings.activeBuildTarget.ToString();

        if (!Directory.Exists(binpath + "/" + platform))
        {
            Directory.CreateDirectory(binpath + "/" + platform);
        }
        string outputPath = string.Empty;

        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.Android:
                outputPath = binpath + "/" + platform + "/" + PlayerSettings.productName + ".apk";
                break;
            case BuildTarget.iOS:
                {

                    outputPath = binpath + "/" + platform + "/" + PlayerSettings.productName + "_xcode";

                    if (!Directory.Exists(outputPath))
                    {
                        Directory.CreateDirectory(outputPath);
                    }
                }
                break;

        }


        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputPath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
        /*
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            //C:\Users\DELAKEY>powershell "D:\Source\The-moon\Assets\Editor\zipper.vbs" "D:\Source\The-moon\Assets\Editor\111" "D:\Source\The-moon\Assets\Editor\111.zip"
            XcodeToZip();
        }*/
    }
    //[MenuItem("Build/Zip")]
    static void XcodeToZip()
    {
        Init();

        string platform = EditorUserBuildSettings.activeBuildTarget.ToString();
        string outputPath = binpath + "/" + platform + "/" + PlayerSettings.productName + "_xcode";


        string script = projectpath + @"\Assets\Editor\zipper.vbs";
        string sourse = outputPath;
        string target = outputPath + @".zip";

        if (File.Exists(target))
        {
            File.Delete(target);
        }

        string sourceName = sourse;
        string targetName = target;
        {
            ProcessStartInfo p = new ProcessStartInfo();
            //first change
            p.FileName = @"C:\Program Files\7-Zip\7z.exe";
            //second change
            p.Arguments = "a -tzip \"" + targetName + "\" \"" + sourceName + "\" -mx=0";
            Process x = Process.Start(p);
            x.WaitForExit();
        }
        {/*
            ProcessStartInfo p = new ProcessStartInfo();
            //first change
            p.FileName = @"cmd";
            //second change
            p.Arguments = "/C copy \"" + targetName.Replace("/","\\") + "\" \"" + @"\\imac-admin\Desktop\" + PlayerSettings.productName + "_xcode.zip\"";
            UnityEngine.Debug.Log(p.Arguments);
            Process x = Process.Start(p);
            x.WaitForExit();*/
            Upload();
        }
    }

    static long Lost;

    [MenuItem("Build/Only Upload")]
    static void Upload()
    {
        Init();

        string platform = EditorUserBuildSettings.activeBuildTarget.ToString();
        string outputPath = binpath + "/" + platform + "/" + PlayerSettings.productName + "_xcode";

        string target = outputPath + @".zip";
        string boundary = "----------" + DateTime.Now.Ticks.ToString("x");

        var file = File.Open(target, FileMode.Open);
        Lost = file.Length + 200;

        WebRequest request = WebRequest.Create("http://192.168.1.91:3000/upload/");
        ((HttpWebRequest)request).AllowWriteStreamBuffering = false;

        request.Method = "POST";
        request.ContentType = "multipart/form-data; boundary=" + boundary;
        request.ContentLength = Lost;
        Stream dataStream = request.GetRequestStream();

        WriteToStream(dataStream, "--" + boundary + "\r\n");
        WriteToStream(dataStream, "Content-Disposition: form-data; name=\"file\"; filename=\"" + PlayerSettings.productName + "_xcode.zip\"\r\n");
        WriteToStream(dataStream, "Content-Type: text/plain; charset=utf-8\r\n");

        WriteToStream(dataStream, "\r\n");

        int count = -1;

        byte[] buffer = new byte[1024 * 1024];
        while (count != 0)
        {
            count = file.Read(buffer, 0, 1024 * 1024);
            dataStream.Write(buffer, 0, count);
            Lost -= count;
        }

        WriteToStream(dataStream, "\r\n");

        WriteToStream(dataStream, "--" + boundary + "--");
        for (int i = 0; i < Lost; i++)
            dataStream.WriteByte(0);

        WebResponse response = request.GetResponse();
        dataStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(dataStream);
        string responseFromServer = reader.ReadToEnd();
        reader.Close();
        response.Close();
        //Console.WriteLine(responseFromServer);
    }
    private static void WriteToStream(Stream s, string txt)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(txt);
        s.Write(bytes, 0, bytes.Length);
    }



    [MenuItem("Build/Only Bundle")]
    static void BuildBundle()
    {
        Init();

        string projectpath = Application.dataPath.Replace("Assets", "");

        string binpath = projectpath + "bin";
        if (!Directory.Exists(binpath))
        {
            Directory.CreateDirectory(binpath);
        }

        string platform = EditorUserBuildSettings.activeBuildTarget.ToString();

        string outputPath = binpath + "/" + platform + "/" + "AssetBundles";


        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }


        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        //BuildPipeline.BuildAssetBundles()
        //Debug.Log(outputPath);
    }
}