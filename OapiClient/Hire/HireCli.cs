using LarkSuite;
using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;
using Trivial.Collection;
using Trivial.CommandLine;
using Trivial.Data;
using Trivial.Text;
using Trivial.Web;

namespace LarkSuite.CommandLine;

public class LarkHireCommandVerb : BaseCommandVerb
{
    /// <summary>
    /// Gets the description of the command.
    /// </summary>
    public static string Description => "Get details of interview and candidate.";

    /// <summary>
    /// Gets a value indicating whether need enable evaluation.
    /// </summary>
    protected virtual bool IsEvaluationEnabled { get; }

    /// <summary>
    /// Gets the days.
    /// </summary>
    protected virtual int LatestDays => 5;

    /// <inheritdoc />
    protected override async Task OnProcessAsync(CancellationToken cancellationToken = default)
    {
        var console = CurrentConsole;
        var verb = Arguments.Verb;
        var verbStr = verb.Count > 0 ? verb[0]?.Trim()?.ToLowerInvariant() : null;
        var consoleKey = ConsoleKey.Spacebar;
        if (!string.IsNullOrEmpty(verbStr))
        {
            consoleKey = verbStr.Trim().ToLowerInvariant() switch
            {
                "latest" => ConsoleKey.L,
                "day" => ConsoleKey.D,
                "talent" => ConsoleKey.T,
                "quit" => ConsoleKey.Q,
                _ => ConsoleKey.Spacebar,
            };
        }

        if (consoleKey == ConsoleKey.Spacebar)
        {
            console.Write("Search [L]atest, on specific [D]ay, or by specific [T]alent?  ");
            consoleKey = console.ReadKey().Key;
            console.WriteLine();
        }

        switch (consoleKey)
        {
            case ConsoleKey.L:
            case ConsoleKey.M:
            case ConsoleKey.Enter:
            case ConsoleKey.Spacebar:
                await GetInterviewsAsync(DateTime.Now.AddDays(-Math.Abs(LatestDays)).Date, DateTime.Now, cancellationToken);
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

    /// <summary>
    /// Processes on evaluation.
    /// </summary>
    /// <param name="interview">The interview information.</param>
    /// <param name="cancellationToken">A cancellation id to observe while waiting for the task to complete.</param>
    /// <returns>The async task.</returns>
    protected virtual Task OnEvaluateAsync(LarkHireInterviewInfo interview, CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    /// <summary>
    /// Gets the interview collection.
    /// </summary>
    /// <param name="start">The start date to search.</param>
    /// <param name="end">The end date to search.</param>
    /// <param name="cancellationToken">A cancellation id to observe while waiting for the task to complete.</param>
    /// <returns>The interview information collection.</returns>
    public async Task<IReadOnlyList<LarkHireInterviewInfo>> GetInterviewsAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
    {
        var larkApi = LarkApi.DefaultInstance;
        var console = CurrentConsole;
        var resp = await GetInterviewsAsync(larkApi, new LarkInterviewOptions
        {
            Start = start,
            End = end,
        }, cancellationToken);
        if (resp is null)
        {
            LarkCliUtils.WriteEmpty(console);
            return [];
        }

        resp = resp.Take(99).ToList();
        var ids = LarkCliUtils.WriteLine(console, resp);
        console.WriteLine();
        console.WriteLine("Please type the index of interview ID to get interview minutes.");
        var id = ReadInterviewId(ids);
        if (LarkCliUtils.IsToExit(id)) return resp;
        var interviewInfo = GetInterviewInfo(id, resp);
        if (interviewInfo is null) return resp;
        var applicationId = interviewInfo.ApplicationId;
        JsonObjectNode? applicationBasic = null;
        if (!string.IsNullOrWhiteSpace(applicationId))
        {
            var applicationInfo = await larkApi.GetHireApplicationAsync(applicationId);
            applicationBasic = applicationInfo?.Data?.TryGetObjectValue("basic_info");
            var talentInfo = applicationInfo?.Data?.TryGetObjectValue("talent");
            if (talentInfo is not null)
            {
                console.WriteLine(LarkCliUtils.ItalicText(), "Talent Info");
                console.WriteLine(LarkCliUtils.BoldText(), talentInfo.TryGetStringTrimmedValue("name"));
                console.WriteLine(ConsoleColor.Yellow, talentInfo.TryGetStringTrimmedValue("id"));
                console.WriteLine();
            }

            var jobInfo = applicationInfo?.Data?.TryGetObjectValue("job");
            if (jobInfo is not null)
            {
                console.WriteLine(LarkCliUtils.ItalicText(), "Job Info");
                console.WriteLine(LarkCliUtils.BoldText(), jobInfo.TryGetStringTrimmedValue("name"));
                console.WriteLine(ConsoleColor.Yellow, jobInfo.TryGetStringTrimmedValue("id"));
                console.WriteLine();
            }
        }

        WriteInterviewResultLine(interviewInfo);
        while (true)
        {
            console.Write("Select action: Get [T]alent info, Get [J]ob info, interview [M]inutes, ");
            if (IsEvaluationEnabled) console.Write("[E]valuation, ");
            console.Write("or [Q]uit. ");
            var key = console.ReadKey();
            console.WriteLine();
            switch (key.Key)
            {
                case ConsoleKey.E:
                    if (IsEvaluationEnabled)
                    {
                        await OnEvaluateAsync(interviewInfo);
                    }
                    else
                    {
                        console.WriteLine(ConsoleColor.Yellow, "Not support.");
                    }

                    console.WriteLine();
                    break;
                case ConsoleKey.M:
                    await GetInterviewMinutesAsync(id);
                    console.WriteLine();
                    break;
                case ConsoleKey.T:
                case ConsoleKey.I:
                    {
                        var talentId = applicationBasic?.TryGetStringTrimmedValue("talent_id", true);
                        if (talentId == null)
                        {
                            LarkCliUtils.WriteEmpty(console);
                            break;
                        }

                        var talent = await larkApi.GetHireTalentAsync(talentId, cancellationToken);
                        if (talent?.Data is not null) console.WriteLine(talent.Data);
                        else LarkCliUtils.WriteEmpty(console);
                    }

                    break;
                case ConsoleKey.J:
                case ConsoleKey.A:
                    {
                        var jobId = applicationBasic?.TryGetStringTrimmedValue("job_id", true);
                        if (jobId == null)
                        {
                            LarkCliUtils.WriteEmpty(console);
                            break;
                        }

                        var job = await larkApi.GetHireJobAsync(jobId, cancellationToken);
                        var jobInfo = job?.Data?.TryGetObjectValue("basic_info");
                        if (jobInfo is null)
                        {
                            LarkCliUtils.WriteEmpty(console);
                            break;
                        }

                        console.WriteLine(LarkCliUtils.BoldText(), jobInfo.TryGetStringValue("title"));
                        console.WriteLine(ConsoleColor.Yellow, jobInfo.TryGetStringValue("id"));
                        console.WriteLine();
                        console.WriteLine(LarkCliUtils.ItalicText(), "Description");
                        console.WriteLine(jobInfo.TryGetStringValue("description"));
                        console.WriteLine();
                        console.WriteLine(LarkCliUtils.ItalicText(), "Requirement");
                        console.WriteLine(jobInfo.TryGetStringValue("requirement"));
                        console.WriteLine();
                    }

                    break;
                case ConsoleKey.X:
                case ConsoleKey.Q:
                default:
                    console.WriteLine();
                    return resp;
            }
        }
    }

    public async Task<IReadOnlyList<LarkInterviewMinuteInfo>> GetTalentInterviewAsync(CancellationToken cancellationToken = default)
    {
        var larkApi = LarkApi.DefaultInstance;
        var console = CurrentConsole;
        var talent = await GetTalentAsync(CurrentConsole, larkApi, cancellationToken);
        var basic = talent?.BasicInfo;
        if (basic is null) return [];
        var task = larkApi.ListInterviewsAsync(new LarkInterviewByTelentOptions
        {
            Id = talent!.Id
        }, cancellationToken);
        console.WriteLine();
        if (basic is not null) console.WriteLine(talent);
        console.WriteLine(LarkCliUtils.ItalicText(), "Interviews");
        var arr = await task;
        if (LarkCliUtils.WriteEmpty(console, arr)) return [];
        var interviews = arr.Data.FirstOrDefault()?.List;
        if (interviews is null || interviews.Count < 1)
        {
            LarkCliUtils.WriteEmpty(console);
            return [];
        }

        var ids = LarkCliUtils.WriteLine(console, interviews);
        console.WriteLine();
        console.WriteLine("Please type the index or ID of interview to show minutes.");
        var id = ReadInterviewId(ids);
        if (LarkCliUtils.IsToExit(id)) return [];
        var interviewInfo = GetInterviewInfo(id, interviews);
        WriteInterviewResultLine(interviewInfo);
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
            LarkApi.DefaultInstance.GetInterviewMinutesAsync,
            LarkApi.DefaultInstance.GetInterviewMinutesAsync, 
            LarkCliUtils.WriteLine,
            50,
            50,
            cancellationToken);
        console.WriteLine($"Total {minutes.Count} records.");
        return minutes.Data;
    }

    private LarkHireInterviewInfo? GetInterviewInfo(string? id, IEnumerable<LarkHireInterviewInfo> arr)
        => GetInterviewInfo(CurrentConsole, id, arr);

    private void WriteInterviewResultLine(LarkHireInterviewInfo? item)
    {
        if (item is null) return;
        var console = CurrentConsole;
        var records = item.Records;
        if (records is null || records.Count < 1) return;
        console.WriteLine();
        console.WriteLine(LarkCliUtils.ItalicText(), "Interview records");
        foreach (var record in records)
        {
            console.Write(ConsoleColor.Blue, "· ");
            console.Write(ConsoleColor.DarkGray, record.Id);
            console.Write(" \t");
            console.WriteLine(record.Interviewer.GetName() ?? "?");
            var score = record.Score ?? [];
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

    private string? ReadInterviewId(IList<string> ids)
        => LarkCliUtils.ReadId(CurrentConsole, "Hire\\Interview", ids);

    private static string? ReadTalentId(StyleConsole console, IList<string>? ids)
        => ids is null ? LarkCliUtils.ReadLine(console, "Hire\\Talent") : LarkCliUtils.ReadId(console, "Hire\\Talent", ids);

    private static LarkHireInterviewInfo? GetInterviewInfo(StyleConsole console, string? id, IEnumerable<LarkHireInterviewInfo>? arr)
    {
        if (string.IsNullOrWhiteSpace(id) || arr is null) return null;
        foreach (var item in arr)
        {
            if (item?.Id == id) return item;
        }

        return null;
    }

    private static async Task<List<LarkHireInterviewInfo>?> GetInterviewsAsync(LarkApi? larkApi, LarkInterviewOptions? options, CancellationToken cancellationToken)
    {
        var resp = await larkApi.ListInterviewsAsync(options, new(50), cancellationToken);
        if (resp.IsError || resp.Data is null) return null;
        while (resp.HasNextPage)
        {
            var items = await larkApi.ListInterviewsAsync(resp, 50);
            if (items is null) break;
        }

        return resp.Data.Reverse().ToList();
    }

    public static Task<LarkHireTalentInfo?> GetTalentAsync(StyleConsole? console, LarkApi? larkApi, CancellationToken cancellationToken = default)
        => GetTalentAsync(console, larkApi, null, cancellationToken);

    public static async Task<LarkHireTalentInfo?> GetTalentAsync(StyleConsole? console, LarkApi? larkApi, LarkInterviewOptions? options, CancellationToken cancellationToken)
    {
        larkApi ??= LarkApi.DefaultInstance;
        console ??= StyleConsole.Default;
        string? keyword = null;
        if (options is not null)
        {
            console.Write("Loading latest interviews…");
            var interviews = await GetInterviewsAsync(larkApi, options, cancellationToken);
            console.Clear(StyleConsole.RelativeAreas.Line);
            console.BackspaceToBeginning();
            if (interviews is not null && interviews.Count > 0)
            {
                interviews = interviews.Take(60).ToList();
                var ids = LarkCliUtils.WriteLine(console, interviews, true);
                console.WriteLine("Above are the latest interviews. Please type the index to continue. You can also type the ID, name or keyword of talent.");
                keyword = ReadTalentId(console, ids)!;
                var interviewInfo = GetInterviewInfo(console, keyword, interviews);
                var info2 = await GetTalentAsync(larkApi, interviewInfo, cancellationToken);
                if (info2?.Data is null || info2.IsError)
                {
                    console.WriteLine("Load talent failed from the interview.");
                    keyword = null;
                }
                else
                {
                    return info2.Data.BasicInfo is not null ? info2.Data : null;
                }
            }
        }

        if (string.IsNullOrWhiteSpace(keyword))
        {
            console.WriteLine("Search a talent. Please type the ID, name or keyword.");
            keyword = ReadTalentId(console, null)!;
        }

        if (string.IsNullOrEmpty(keyword)) return null;
        var info = await larkApi.GetHireTalentAsync(keyword, cancellationToken);
        var talent = info?.Data;
        if (talent?.BasicInfo is not null) return talent;
        var talents = await larkApi.SearchHireTalentsAsync(keyword);
        if (LarkCliUtils.WriteEmpty(console, talents)) return null;
        var selection = new List<SelectionItem<string>>();
        var dict = new Dictionary<string, LarkHireTalentInfo>();
        foreach (var talentInfo in talents.Data)
        {
            var talentId = talentInfo.Id ?? talentInfo.IdInOldVersion;
            var talentName = talentInfo?.BasicInfo?.Name;
            if (talentId == null || talentName == null) continue;
            dict[talentId] = talentInfo!;
            selection.Add(new(talentName, talentId));
        }

        LarkCliUtils.WriteOrderedLine(console, selection);
        console.WriteLine("Please type the index.");
        var id2 = ReadTalentId(console, null);
        if (LarkCliUtils.IsToExit(id2) || string.IsNullOrWhiteSpace(id2) || !dict.TryGetValue(id2, out talent)) return null;
        info = await larkApi.GetHireTalentAsync(talent.Id ?? talent.IdInOldVersion, cancellationToken);
        return info?.Data?.BasicInfo is not null ? info.Data : talent;
    }

    public static async Task<LarkResponseBody<LarkHireTalentInfo>?> GetTalentAsync(LarkApi larkApi, LarkHireInterviewInfo interviewInfo, CancellationToken cancellationToken = default)
    {
        var applicationId = interviewInfo?.ApplicationId;
        if (string.IsNullOrWhiteSpace(applicationId)) return null;
        larkApi ??= LarkApi.DefaultInstance;
        var applicationInfo = await larkApi.GetHireApplicationAsync(applicationId);
        return await GetTalentByApplicationAsync(larkApi, applicationInfo?.Data, cancellationToken);
    }

    public static async Task<LarkResponseBody<LarkHireTalentInfo>?> GetTalentByApplicationAsync(LarkApi larkApi, JsonObjectNode? application, CancellationToken cancellationToken = default)
    {
        var talentId = application?.TryGetObjectValue("basic_info")?.TryGetStringTrimmedValue("talent_id", true);
        if (talentId is null) return null;
        var resp = await larkApi.GetHireTalentAsync(talentId, cancellationToken);
        return resp;
    }

    public static async Task<LarkHireTalentInfo?> GetTalentByApplicationAsync(LarkApi larkApi, JsonObjectNode? application, DataCacheCollection<LarkHireTalentInfo> cache, CancellationToken cancellationToken = default)
    {
        var talentId = application?.TryGetObjectValue("basic_info")?.TryGetStringTrimmedValue("talent_id", true);
        if (talentId is null) return null;
        if (cache is not null && cache.TryGet(talentId, out var data) && data is not null) return data;
        var resp = await larkApi.GetHireTalentAsync(talentId, cancellationToken);
        return resp?.Data;
    }

    public static async Task<LarkHireTalentInfo?> GetTalentByApplicationAsync(LarkApi larkApi, string applicationId, DataCacheCollection<LarkHireTalentInfo> cache, CancellationToken cancellationToken = default)
    {
        var application = await larkApi.GetHireApplicationAsync(applicationId, cancellationToken);
        if (application?.Data is null || application.IsError) return null;
        return await GetTalentByApplicationAsync(larkApi, application.Data, cache, cancellationToken);
    }

    public static async Task<LarkResponseBody?> GetJobByApplicationAsync(LarkApi larkApi, JsonObjectNode? application, CancellationToken cancellationToken = default)
    {
        var jobId = application?.TryGetObjectValue("basic_info")?.TryGetStringTrimmedValue("job_id", true);
        if (jobId is null) return null;
        var resp = await larkApi.GetHireJobAsync(jobId, cancellationToken);
        return resp;
    }

    public static async Task<JsonObjectNode?> GetJobByApplicationAsync(LarkApi larkApi, JsonObjectNode? application, DataCacheCollection<JsonObjectNode> cache, CancellationToken cancellationToken = default)
    {
        var jobId = application?.TryGetObjectValue("basic_info")?.TryGetStringTrimmedValue("job_id", true);
        if (jobId is null) return null;
        if (cache is not null && cache.TryGet(jobId, out var data) && data is not null) return data;
        var resp = await larkApi.GetHireJobAsync(jobId, cancellationToken);
        return resp?.Data;
    }

    public static async Task<JsonObjectNode?> GetJobByApplicationAsync(LarkApi larkApi, string applicationId, DataCacheCollection<JsonObjectNode> cache, CancellationToken cancellationToken = default)
    {
        var application = await larkApi.GetHireApplicationAsync(applicationId, cancellationToken);
        if (application?.Data is null || application.IsError) return null;
        return await GetJobByApplicationAsync(larkApi, application.Data, cache, cancellationToken);
    }
}

public static partial class LarkCliUtils
{
    public static List<string> WriteLine(StyleConsole console, LarkResponsePagingBody<LarkHireInterviewInfo> resp, bool hideId = false)
    {
        if (WriteEmpty(console, resp)) return [];
        return WriteLine(console, resp.Data, hideId);
    }

    public static List<string> WriteLine(StyleConsole console, IEnumerable<LarkHireInterviewInfo> resp, bool hideId = false)
    {
        var ids = new List<string>();
        foreach (var interviewItem in resp)
        {
            var interviewId = interviewItem?.Id;
            if (interviewId is null) continue;
            var i = ids.Count;
            ids.Add(interviewId);
            var sb = new StringBuilder();
            var start = interviewItem!.BeginDate;
            if (start.HasValue)
            {
                sb.Append(start.Value.ToString("f"));
                var end = interviewItem.EndDate;
                if (end.HasValue)
                {
                    sb.Append(" → ");
                    sb.Append(start.Value.Date == end.Value.Date
                        ? end.Value.ToShortTimeString()
                        : end.Value.ToString("f"));
                }

                sb.Append(" ");
            }

            var stageName = interviewItem.Stage?.GetName();
            if (stageName is not null)
            {
                sb.Append(stageName);
                sb.Append(" ");
            }

            var contact = interviewItem.GetInterviewers().FirstOrDefault()?.GetName() ?? interviewItem.ContactUser?.GetName();
            if (!string.IsNullOrWhiteSpace(contact))
            {
                sb.Append("by ");
                sb.Append(contact);
                sb.Append(" ");
            }

            sb.Append("| ");
            sb.Append(interviewItem.GetStateString());
            WriteOrderedLine(console, i, hideId ? string.Empty : interviewId, sb.ToString(), true);
        }

        return ids;
    }

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

    public static void WriteLine(this StyleConsole console, LarkHireTalentInfo talent)
    {
        console ??= StyleConsole.Default;
        if (talent is null) return;
        var basic = talent.BasicInfo;
        if (basic is not null)
        {
            console.WriteLine(BoldText(), basic.Name);
            console.WriteLine(ConsoleColor.Yellow, talent.Id);
            console.WriteLine();
            var phone = basic.PhoneNumber;
            if (phone is not null)
            {
                var phoneRegion = basic.PhoneNumberRegionCode;
                WritePropertyLine(console, "Phone", phoneRegion is null ? phone : $"+{phoneRegion} {phone}");
            }

            WritePropertyLine(console, "Email", basic.Email);
            WritePropertyLine(console, "Gender", basic.Gender.ToString());
            var birthday = basic.Birthday;
            if (birthday.HasValue) WritePropertyLine(console, "Birthday", birthday.Value.Date.ToShortDateString());
            console.WriteLine();
        }

        var career = talent.WorkingInfo;
        if (career is not null && career.Count == 1)
        {
            var xp = career[0];
            if (xp is null || (string.IsNullOrWhiteSpace(xp?.Id) && string.IsNullOrWhiteSpace(xp?.Name)))
                career = null;
        }

        if (career is not null && career.Count > 0)
        {
            console.WriteLine(ItalicText(), "Working Experience");
            foreach (var xp in career)
            {
                WriteLine(console, xp, true);
            }

            console.WriteLine();
        }

        career = talent.InternInfo;
        if (career is not null && career.Count == 1)
        {
            var xp = career[0];
            if (xp is null || (string.IsNullOrWhiteSpace(xp?.Id) && string.IsNullOrWhiteSpace(xp?.Name)))
                career = null;
        }

        if (career is not null && career.Count > 0)
        {
            console.WriteLine(ItalicText(), "Intern Experience");
            foreach (var xp in career)
            {
                WriteLine(console, xp, true);
            }

            console.WriteLine();
        }

        var edu = talent.EducationInfo;
        if (edu is not null && edu.Count > 0)
        {
            console.WriteLine(ItalicText(), "Education");
            foreach (var xp in edu)
            {
                console.Write(ConsoleColor.Blue, "· ");
                console.WriteLine($"{xp.StartDate ?? "?"} → {xp.EndDate ?? "?"}");
                console.Write(ConsoleColor.Blue, "  ");
                console.Write(xp.Name ?? "?");
                console.Write(" \t ");
                console.WriteLine(xp.Major);
            }

            console.WriteLine();
        }
    }

    public static void WriteLine(this StyleConsole console, LarkHireTalentWorkingInfo xp, bool bullet = false)
    {
        if (string.IsNullOrWhiteSpace(xp?.Id) && string.IsNullOrWhiteSpace(xp?.Name)) return;
        if (bullet)
        {
            console.Write(ConsoleColor.Blue, "· ");
            console.WriteLine($"{xp.StartDate ?? "?"} → {xp.EndDate ?? "?"}");
            console.Write(ConsoleColor.Blue, "  ");
            console.Write(xp.Name ?? "?");
            console.Write(" \t ");
            console.WriteLine(xp.JobTitle);
        }
        else
        {
            console.WriteLine(BoldText(), xp.Name ?? "?");
            console.Write(' ');
            console.WriteLine($"{xp.StartDate ?? "?"} → {xp.EndDate ?? "?"}");
            if (!string.IsNullOrWhiteSpace(xp.JobTitle))
            {
                console.Write(' ');
                console.WriteLine(xp.JobTitle);
            }

            if (!string.IsNullOrWhiteSpace(xp.Description)) console.WriteLine(xp.Description);
        }
    }
}
