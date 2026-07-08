using LarkSuite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using LarkSuite.OapiModels;
using Trivial.CommandLine;
using Trivial.Text;
using System.ComponentModel.DataAnnotations;
using Trivial.Web;

namespace LarkSuite.CommandLine;

public class LarkHireCommandVerb : BaseCommandVerb
{
    public static string Description => "Get details of interview and candidate.";

    protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
    {
        var console = CurrentConsole;
        console.Write("Search [L]atest, on specific [D]ay, or by [T]alent ID?  ");
        var c = console.ReadKey();
        console.WriteLine();
        switch (c.Key)
        {
            case ConsoleKey.L:
            case ConsoleKey.M:
            case ConsoleKey.Enter:
            case ConsoleKey.Spacebar:
                await GetInterviewsAsync(DateTime.Now.AddMonths(-1).Date, DateTime.Now, cancellationToken);
                break;
            case ConsoleKey.D:
                {
                    console.WriteLine("Please type the date in YYYY-MM-DD format.");
                    var s = LarkCliUtils.ReadLine(console, "Date")!;
                    var date = WebFormat.ParseDate(s);
                    if (!date.HasValue && !string.IsNullOrWhiteSpace(s) && s.Length < 6)
                    {
                        var i = s.IndexOf('-');
                        if (i > 0 && s.IndexOf('-', i + 1) < 0)
                        {
                            var m = s[0..i];
                            var d = s[(i + 1)..];
                            if (!string.IsNullOrEmpty(m) && !string.IsNullOrEmpty(d) && int.TryParse(m, out var month) && int.TryParse(d, out var day))
                                date = new DateTime(DateTime.Now.Year, month, day);
                        }
                    }

                    if (date.HasValue)
                    {
                        await GetInterviewsAsync(date.Value.Date, date.Value.Date.AddDays(1), cancellationToken);
                    }
                    else
                    {
                        console.Write(ConsoleColor.Red, "Error");
                        console.WriteLine(" \tParse date failed.");
                        console.WriteLine();
                    }

                    break;
                }
            case ConsoleKey.T:
            case ConsoleKey.I:
                await GetTalentInterviewAsync(cancellationToken);
                break;
        }
    }

    public async Task<IReadOnlyList<JsonObjectNode>> GetInterviewsAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var larkApi = LarkApi.DefaultInstance;
        var console = CurrentConsole;
        var resp = await larkApi.GetInterviews(new LarkInterviewOptions
        {
            Start = start,
            End = end,
        }, new(50));
        var ids = WriteLine(resp);
        console.WriteLine();
        console.WriteLine("Please type the index of interview ID to get interview minutes.");
        var id = ReadInterviewId(ids);
        WriteInterviewResultLine(id, resp.Data);
        await GetInterviewMinutesAsync(id);
        return resp.Data;
    }

    public async Task<IReadOnlyList<LarkInterviewMinuteInfo>> GetTalentInterviewAsync(CancellationToken cancellationToken = default)
    {
        var larkApi = LarkApi.DefaultInstance;
        var console = CurrentConsole;
        console.WriteLine("Please type the talent ID.");
        var id = LarkCliUtils.ReadLine(console, "Hire\\Talent")!;
        if (string.IsNullOrEmpty(id)) return [];
        var task = larkApi.GetInterviews(new LarkInterviewByTelentOptions
        {
            Id = id
        }, cancellationToken);
        var info = await larkApi.GetHireTalent(id, cancellationToken);
        console.WriteLine();
        var basic = info?.Data?.TryGetObjectValue("basic_info");
        if (basic is not null)
        {
            console.WriteLine(LarkCliUtils.BoldText(), basic.TryGetStringValue("name"));
            console.WriteLine(ConsoleColor.Yellow, info.Data.TryGetStringValue("talent_id"));
            console.WriteLine();
            var phone = basic.TryGetStringTrimmedValue("mobile_number", true);
            if (phone is not null)
            {
                var phoneRegion = basic.TryGetStringTrimmedValue("mobile_code", true);
                LarkCliUtils.WritePropertyLine(console, "Phone", phoneRegion is null ? phone : $"+{phoneRegion} {phone}");
            }

            LarkCliUtils.WritePropertyLine(console, "Email", basic.TryGetStringValue("email"));
            LarkCliUtils.WritePropertyLine(console, "Gender", (basic.TryGetInt32Value("gender") ?? 0) switch
            {
                1 => "Male",
                2 => "Female",
                3 => "Other",
                _ => "Unknown",
            });
            var birthday = basic.TryGetDateTimeValue("birthday");
            if (birthday.HasValue) LarkCliUtils.WritePropertyLine(console, "Birthday", birthday.Value.Date.ToShortDateString());
            var career = info.Data.TryGetObjectListValue("career_list", true);
            if (career is not null && career.Count > 0)
            {
                console.WriteLine();
                console.WriteLine(LarkCliUtils.ItalicText(), "Companies");
                foreach (var xp in career)
                {
                    console.Write(ConsoleColor.Blue, "· ");
                    console.WriteLine($"{xp.TryGetStringTrimmedValue("start_time") ?? "?"} → {xp.TryGetStringTrimmedValue("end_time") ?? "?"}");
                    console.Write(ConsoleColor.Blue, "  ");
                    console.Write(xp.TryGetStringTrimmedValue("company_name", true) ?? "?");
                    console.Write(" \t ");
                    console.WriteLine(xp.TryGetStringTrimmedValue("title", true));
                }
            }

            var edu = info.Data.TryGetObjectListValue("education_list");
            if (edu is not null && edu.Count > 0)
            {
                console.WriteLine();
                console.WriteLine(LarkCliUtils.ItalicText(), "Education");
                foreach (var xp in edu)
                {
                    console.Write(ConsoleColor.Blue, "· ");
                    console.WriteLine($"{xp.TryGetStringTrimmedValue("start_time") ?? "?"} → {xp.TryGetStringTrimmedValue("end_time") ?? "?"}");
                    console.Write(ConsoleColor.Blue, "  ");
                    console.Write(xp.TryGetStringTrimmedValue("school_name", true) ?? "?");
                    console.Write(" \t ");
                    console.WriteLine(xp.TryGetStringTrimmedValue("major", true));
                }
            }

            console.WriteLine();
        }

        console.WriteLine(LarkCliUtils.ItalicText(), "Interviews");
        var arr = await task;
        if (LarkCliUtils.WriteEmpty(console, arr)) return [];
        var interviews = arr.Data.FirstOrDefault()?.TryGetObjectListValue("interview_list", true);
        if (interviews is null || interviews.Count < 1)
        {
            LarkCliUtils.WriteEmpty(console);
            return [];
        }

        var ids = WriteLine(interviews);
        console.WriteLine();
        id = ReadInterviewId(ids);
        WriteInterviewResultLine(id, interviews);
        return await GetInterviewMinutesAsync(id);
    }

    public async Task<IReadOnlyList<LarkInterviewMinuteInfo>> GetInterviewMinutesAsync(string? id, CancellationToken cancellationToken = default)
    {
        var console = CurrentConsole;
        if (string.IsNullOrEmpty(id)) return [];
        console.WriteLine(LarkCliUtils.ItalicText(), "Interview minutes");
        var minutes = await LarkCliUtils.WritePagesAsync(
            console,
            new LarkInterviewOptions
            {
                Id = id,
            },
            LarkApi.DefaultInstance.GetInterviewMinutes,
            LarkApi.DefaultInstance.GetInterviewMinutes, 
            LarkCliUtils.WriteLine,
            50,
            50,
            cancellationToken);
        console.WriteLine($"Total {minutes.Count} records.");
        return minutes.Data;
    }

    private string? ReadInterviewId(IList<string> ids)
        => LarkCliUtils.ReadId(CurrentConsole, "Hire\\Interview", ids);

    private void WriteInterviewResultLine(string? id, IReadOnlyList<JsonObjectNode> arr)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        var console = CurrentConsole;
        foreach (var item in arr)
        {
            if (item?.TryGetStringTrimmedValue("id", true) != id) continue;
            var records = item.TryGetObjectListValue("interview_record_list", true);
            if (records is null || records.Count < 1) break;
            console.WriteLine();
            console.WriteLine(LarkCliUtils.ItalicText(), "Interview records");
            foreach (var record in records)
            {
                console.Write(ConsoleColor.Blue, "· ");
                console.Write(ConsoleColor.DarkGray, record.TryGetStringTrimmedValue("id"));
                console.Write(" \t");
                console.WriteLine(LarkCliUtils.GetName(record.TryGetObjectValue("interviewer")?.TryGetObjectValue("name")) ?? "?");
                var score = record.TryGetObjectValue("interview_score") ?? [];
                var scoreText = score.TryGetStringTrimmedValue("zh_description", true) ?? score.TryGetStringTrimmedValue("en_description", true) ?? score.TryGetStringTrimmedValue("zh_name", true) ?? score.TryGetStringTrimmedValue("en_name", true);
                if (scoreText is not null)
                {
                    console.Write("  ");
                    console.WriteLine(scoreText);
                }

                var scoreLevel = score.TryGetInt32Value("level");
                if (scoreLevel.HasValue)
                {
                    console.Write("  Score level: ");
                    console.Write(ConsoleColor.Green, scoreLevel.Value);
                    console.WriteLine('.');
                }
            }

            console.WriteLine();
        }
    }

    private List<string> WriteLine(LarkResponsePagingBody resp)
    {
        var console = CurrentConsole;
        if (LarkCliUtils.WriteEmpty(console, resp)) return [];
        return WriteLine(resp.Data);
    }

    private List<string> WriteLine(IReadOnlyList<JsonObjectNode> resp)
    {
        var console = CurrentConsole;
        var ids = new List<string>();
        foreach (var interviewItem in resp)
        {
            var interviewId = interviewItem?.TryGetStringTrimmedValue("id", true);
            if (interviewId is null) continue;
            var i = ids.Count;
            ids.Add(interviewId);
            var sb = new StringBuilder();
            var start = interviewItem!.TryGetDateTimeValue("begin_time");
            if (start.HasValue)
            {
                sb.Append(start.Value.ToString("f"));
                var end = interviewItem.TryGetDateTimeValue("end_time");
                if (end.HasValue)
                {
                    sb.Append(" → ");
                    sb.Append(start.Value.Date == end.Value.Date
                        ? end.Value.ToShortTimeString()
                        : end.Value.ToString("f"));
                }

                sb.Append(" ");
            }

            var stageName = LarkCliUtils.GetName(interviewItem.TryGetObjectValue("stage")?.TryGetObjectValue("name"));
            if (stageName is not null)
            {
                sb.Append(stageName);
                sb.Append(" ");
            }

            var contact = interviewItem.TryGetObjectValue("contact_user")?.TryGetObjectValue("name");
            if (contact is not null)
            {
                sb.Append("by ");
                sb.Append(LarkCliUtils.GetName(contact));
                sb.Append(" ");
            }

            sb.Append("| ");
            sb.Append((interviewItem.TryGetInt32Value("interview_round_summary") ?? 0) switch
            {
                2 => "Pending",
                3 => "Evaluating",
                4 => "Pass",
                5 => "Fail",
                7 => "Evaluating and pass",
                8 => "Partial evaluated",
                9 => "Evaluating and fail",
                10 => "Evaluated",
                11 or 12 or 13 or 14 => "Partial evaluated",
                15 => "Evaluated",
                16 => "Partial fail",
                17 => "No decision",
                18 => "Evaluated",
                _ => "Unknown state"
            });
            LarkCliUtils.WriteOrderedLine(console, i, interviewId, sb.ToString(), true);
        }

        return ids;
    }
}

public static partial class LarkCliUtils
{
    public static void WriteLine(this StyleConsole console, IReadOnlyList<LarkInterviewMinuteInfo> col)
    {
        foreach (var record in col)
        {
            console.Write(ConsoleColor.Blue, "· ");
            console.Write(record.SpeakerName ?? "?");
            console.Write(ConsoleColor.Green, record.SpeakerRole switch
            {
                LarkInterviewRole.Interviewer => " (interviewer) ",
                LarkInterviewRole.Interviewee => " (interviewee) ",
                _ => " ",
            });
            console.WriteLine(ConsoleColor.Green, record.Time.ToString("f"));
            console.WriteLine(record.Message);
            console.WriteLine();
        }
    }
}
