// See https://aka.ms/new-console-template for more information
using System.Text.Json;
var settings = JsonSerializer.Deserialize<Settings> (File.ReadAllText ("settings.json"));
if (settings == null) return;
string inputFileName = settings.CSVPath;
string pdfDir = settings.PDFDir;
if (inputFileName == null || pdfDir == null || !Directory.Exists (pdfDir)) return;
string? outDir = settings.OutDir;
if (outDir == null) return;
if (!Directory.Exists (outDir)) Directory.CreateDirectory (outDir);
string[] lines = File.ReadAllLines (inputFileName);
List<string> pdfNames = new ();
int startIdx = -1;
foreach (var line in lines) {
   // Routine to gather pdf file names
   if (line.StartsWith ("Order", StringComparison.InvariantCultureIgnoreCase)) {
      int idx = 0;
      foreach (var name in SplitCsvLine (line)) {
         if (name.EndsWith (".pdf", StringComparison.InvariantCultureIgnoreCase)) {
            if (startIdx == -1) startIdx = idx;
            pdfNames.Add (name);
         }
         idx++;
      }
   }
      // Routine to copy pdf files to out folder
   if (line.StartsWith ("CO")) {
      var data = SplitCsvLine (line);
      if (data.Count < pdfNames.Count + startIdx) continue;
      string storeName = data[1];
      string destDir = Path.Combine (outDir, storeName);
      for (int i = 0; i < pdfNames.Count; i++) {
         if (int.TryParse (data[startIdx + i], out int quantity) && quantity > 0) {
            string pdfPath = Path.Combine (pdfDir, pdfNames[i]);
            string destPath = Path.Combine (destDir, pdfNames[i]);
            Console.WriteLine ($"{pdfPath} -> {destPath}");
            // File.Copy (pdfPath, destPath);
         }
      }
   }
}

https://stackoverflow.com/questions/17207269/how-to-properly-split-a-csv-using-c-sharp-split-function
List<string> SplitCsvLine (string s) {
   int i;
   int a = 0;
   int count = 0;
   List<string> str = new ();
   for (i = 0; i < s.Length; i++) {
      switch (s[i]) {
         case ',':
            if ((count & 1) == 0) {
               str.Add (s[a..i]);
               a = i + 1;
            }
            break;
         case '"':
         case '\'': count++; break;
      }
   }
   str.Add (s.Substring (a));
   return str;
}

record Settings (string CSVPath, string PDFDir, string OutDir);
