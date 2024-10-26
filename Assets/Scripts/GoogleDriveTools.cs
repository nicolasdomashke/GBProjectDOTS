using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

public static class GoogleDriveTools
{
    public static void Upload(string obj)
    {
        var file = new UnityGoogleDrive.Data.File {Name = "Stat.json", MimeType = "application/json"};
        GoogleDriveFiles.Create(file, "{\"collectablesFound\":0}").Send().OnDone += file => {Debug.Log("Success"); };;
    }
    public static File Download(string fileId)
    {
        File output = new File();
        GoogleDriveFiles.Download(fileId).Send().OnDone += fileNew => {output = fileNew;};
        return output;
    }
    public static string DownloadFile(string fileName)
    {
        string output = "";

        GoogleDriveFiles.List().Send().OnDone += fileList =>
        {
            var file = fileList.Files.Find(f => f.Name == fileName);

            if (file != null)
            {
                var downloadFile = GoogleDriveTools.Download(file.Id);
                if (downloadFile.Content != null)
                {
                    output = Encoding.UTF8.GetString(downloadFile.Content);
                }
            }
        };
        return output;
    }
}
