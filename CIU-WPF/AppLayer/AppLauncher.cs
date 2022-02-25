using CIU_WPF.Common;
using FluentArgs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIU_WPF.AppLayer
{
    public class AppLauncher
    {
        public enum StartupWindows { Browser, Viewer, Editor }

        public StartupWindows StartupWindow = StartupWindows.Viewer;

        private static AppLauncher s_ins = new AppLauncher();
        public static AppLauncher Instance => s_ins;

        internal void LaunchByArgs(string[] args, Serilog.ILogger ivwLog)
        {
            if (args.Length == 1)
            {
                if (FileUtils.IsImageFilePath(args[0]))
                {
                    ShowViewer(args[0], ivwLog);
                    return;
                }
            }

            FluentArgsBuilder.New()
                .DefaultConfigsWithAppDescription("An app to convert png files to jpg files.")
                .Parameter("-f", "--file")
                    .WithDescription("Input file path")
                    .WithExamples("c:\\input.png")
                    .IsOptionalWithDefault("")
                .Parameter("-d", "--directory")
                    .WithDescription("directory path")
                    .WithExamples("c:\\photos")
                    .IsOptionalWithDefault("")
                .Parameter("-m", "--mode")
                    .WithDescription("Launch mode: browser, viewer, editor")
                    .IsOptionalWithDefault("viewer")
                //.Parameter<ushort>("-q", "--quality")
                //    .WithDescription("Quality of the conversion")
                //    .WithValidation(n => n >= 0 && n <= 100)
                //    .IsOptionalWithDefault(50)
                .Call(mode => file => dir =>
                {

                    if (mode == "editor")
                    {
                        ShowEditor(file);
                        
                    }
                    else if (mode == "viewer")
                    {
                        ShowViewer(file, ivwLog);
                    }
                    else
                    {
                        ShowBrowser(dir);
                    }
                    /* ... */
                    //Console.WriteLine($"Convert {inputFile} to {outputFile} with quality {quality}...");
                    /* ... */
                    return Task.CompletedTask;
                })
                .ParseAsync(args);
        }

        internal void OnEditorClosed()
        {
            if (wndViewer != null)
            {
                wndViewer.Show();
                return;
            }
            if (wndBrowser != null)
            {
                wndBrowser.Show();
                return;
            }
            //App.Current.Shutdown();
        }

        ImageBrowserWindow wndBrowser;
        internal void ShowBrowser(string dir)
        {
            if (wndBrowser == null)
                wndBrowser = new ImageBrowserWindow();
            wndBrowser.Browse(dir);
        }

        ImageViewerWindow wndViewer;
        internal void ShowViewer(string file, Serilog.ILogger ivwLog)
        {
            if (wndViewer == null)
                wndViewer = new ImageViewerWindow(ivwLog);
            wndViewer.ShowFile(file);
        }

        ImageEditWindow wndEditor;
        internal void ShowEditor(string file)
        {
            if (wndEditor == null)
                wndEditor = new ImageEditWindow();
            wndEditor.ShowFile(file);
            if (wndViewer != null)
            {
                wndViewer.Close();
                wndViewer = null;
            }
        }
    }
}
