using System.Collections.Generic;
using System.Text;

public static class CsvUtil
{
    // CSV 전체를 행 리스트로 변환. 따옴표/줄바꿈 포함 필드 처리.
    public static List<string[]> ReadRows(string csvText)
    {
        var rows = new List<string[]>();
        var row = new List<string>();
        var sb = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < csvText.Length; i++)
        {
            char c = csvText[i];

            if (c == '\"')
            {
                if (inQuotes && i + 1 < csvText.Length && csvText[i + 1] == '\"')
                { sb.Append('\"'); i++; }
                else { inQuotes = !inQuotes; }
            }
            else if (c == ',' && !inQuotes)
            {
                row.Add(sb.ToString()); sb.Clear();
            }
            else if ((c == '\n' || c == '\r') && !inQuotes)
            {
                if (c == '\r' && i + 1 < csvText.Length && csvText[i + 1] == '\n') i++; // CRLF
                row.Add(sb.ToString()); sb.Clear();
                if (row.Count > 1 || row[0].Length > 0) rows.Add(row.ToArray());
                row.Clear();
            }
            else sb.Append(c);
        }
        // 마지막 셀
        if (sb.Length > 0 || row.Count > 0) { row.Add(sb.ToString()); rows.Add(row.ToArray()); }

        return rows;
    }

    // 헤더 기반 Dictionary로 맵핑
    public static List<Dictionary<string, string>> Parse(string csvText)
    {
        var rows = ReadRows(csvText);
        if (rows.Count == 0) return new List<Dictionary<string, string>>();
        var headers = rows[0];
        var list = new List<Dictionary<string, string>>();
        for (int i = 1; i < rows.Count; i++)
        {
            var dict = new Dictionary<string, string>();
            var r = rows[i];
            for (int c = 0; c < headers.Length && c < r.Length; c++)
                dict[headers[c]] = r[c];
            list.Add(dict);
        }
        return list;
    }
}