﻿using System;
using System.IO;
using System.Data;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Collections.Generic;
using Microsoft.Reporting.WebForms;

namespace Binaria.WMSTek.WebClient.Reports.Dispatch
{
public class Demo : IDisposable
{
    private int m_currentPageIndex;
    private IList<Stream> m_streams;

    private DataTable LoadSalesData()
    {
        DataSet dataSet = new DataSet();
        dataSet.ReadXml("data.xml");
        return dataSet.Tables[0];
    }

    private Stream CreateStream(string name, string fileNameExtension,
        Encoding encoding,
            string mimeType, bool willSeek)
    {
        Stream stream = new FileStream(name + "." + fileNameExtension, 
            FileMode.Create);
        m_streams.Add(stream);
        return stream;
    }

    private void Export(LocalReport report)
    {
        string deviceInfo =
          "<DeviceInfo>" +
          "  <OutputFormat>EMF</OutputFormat>" + 
          "  <PageWidth>8.5in</PageWidth>" +
          "  <PageHeight>11in</PageHeight>" +
          "  <MarginTop>0.25in</MarginTop>" +
          "  <MarginLeft>0.25in</MarginLeft>" +
          "  <MarginRight>0.25in</MarginRight>" +
          "  <MarginBottom>0.25in</MarginBottom>" +
          "</DeviceInfo>";
        Warning[] warnings;
        m_streams = new List<Stream>();
        report.Render("Image", deviceInfo, CreateStream, out warnings);

        foreach (Stream stream in m_streams)
            stream.Position = 0;
    }

    private void PrintPage(object sender, PrintPageEventArgs ev)
    {
        Metafile pageImage = new Metafile(m_streams[m_currentPageIndex]);
        ev.Graphics.DrawImage(pageImage, 0, 0);

        m_currentPageIndex++;
        ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
    }

    private void Print()
    {
        const string printerName = "Microsoft Office Document Image Writer";

        if (m_streams == null || m_streams.Count == 0)
            return;

        PrintDocument printDoc = new PrintDocument(); 
        printDoc.PrinterSettings.PrinterName = printerName;
        if (!printDoc.PrinterSettings.IsValid)
        {
            string msg = String.Format("Can't find printer \"{0}\".", 
                printerName);
            Console.WriteLine(msg);
            return;
        }
        printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
        printDoc.Print();
    }

    private void Run()
    {
        LocalReport report = new LocalReport();
        report.ReportPath = "Report.rdlc";
        report.DataSources.Add(new ReportDataSource("Sales", 
            LoadSalesData()));

        Export(report);

        m_currentPageIndex = 0;
        Print();
    }

    public void Dispose()
    {
        if (m_streams != null)
        {
            foreach (Stream stream in m_streams)
                stream.Close();
            m_streams = null;
        }
    }

    public static int Main(string[] args) 
    {
        using (Demo demo = new Demo())
        {
            demo.Run();
        }
        return 0;
    }
}
}
