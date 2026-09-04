using LarkSuite.Docs;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using Trivial.Collection;
using Trivial.Text;

namespace LarkSuite.OapiModels;

/// <summary>
/// The utilities of Lark doc content.
/// </summary>
public static partial class LarkApiUtils
{
    /// <summary>
    /// Gets the job application by identifier.
    /// </summary>
    /// <param name="col">The job application collection.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>The job application with the specific identifier; or null, if not found.</returns>
    public static LarkHireApplicationInfo? GetById(this IEnumerable<LarkHireApplicationInfo> col, string id)
    {
        if (col is null || string.IsNullOrWhiteSpace(id)) return null;
        foreach (var item in col)
        {
            if (item?.Info?.Id == id) return item;
        }

        return null;
    }

    /// <summary>
    /// Gets the job application by identifier.
    /// </summary>
    /// <param name="col">The job application collection.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>The job application with the specific identifier; or null, if not found.</returns>
    public static LarkHireApplicationBasicInfo? GetById(this IEnumerable<LarkHireApplicationBasicInfo> col, string id)
    {
        if (col is null || string.IsNullOrWhiteSpace(id)) return null;
        foreach (var item in col)
        {
            if (item?.Id == id) return item;
        }

        return null;
    }

    /// <summary>
    /// Gets the job interview by identifier.
    /// </summary>
    /// <param name="col">The job interview collection.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>The job interview with the specific identifier; or null, if not found.</returns>
    public static LarkHireInterviewInfo? GetById(this IEnumerable<LarkHireInterviewInfo> col, string id)
    {
        if (col is null || string.IsNullOrWhiteSpace(id)) return null;
        foreach (var item in col)
        {
            if (item?.Id == id) return item;
        }

        return null;
    }

    /// <summary>
    /// Gets the job talent by identifier.
    /// </summary>
    /// <param name="col">The job talent collection.</param>
    /// <param name="id">The identifier.</param>
    /// <returns>The job talent with the specific identifier; or null, if not found.</returns>
    public static LarkHireTalentInfo? GetById(this IEnumerable<LarkHireTalentInfo> col, string id)
    {
        if (col is null || string.IsNullOrWhiteSpace(id)) return null;
        foreach (var item in col)
        {
            if (item?.Id == id) return item;
        }

        return null;
    }

    public static bool Add(IList<LarkIdNameInfo> list, LarkIdNameInfo? item)
    {
        if (list is null) return false;
        var id = item?.Id?.Trim();
        if (string.IsNullOrEmpty(id)) return false;
        foreach (var ele in list)
        {
            if (ele?.Id == id) return false;
        }

        list.Add(item!);
        return true;
    }
}
