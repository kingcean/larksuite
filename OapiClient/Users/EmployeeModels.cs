using LarkSuite.OapiModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using Trivial.Data;

namespace LarkSuite.Users;

public class LarkEmployeeInfo : IIdPropertyModel, INamePropertyModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("avatarUrl")]
    public string? AvatarUrl { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("info")]
    public LarkEmployeePersonalInfo? Info { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("employment")]
    public LarkEmployeeContractInfo? Employment { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("job")]
    public LarkEmployeeJobInfo? Job { get; set; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
        => $"{Name ?? "?"} ({Id}) {Job?.Title}";
}

public class LarkEmployeePersonalInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("legalName")]
    public string? LegalName { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("birthday")]
    public string? Birthday { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("gender")]
    public LarkGender Gender { get; set; } = LarkGender.Unknown;

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
        => string.IsNullOrWhiteSpace(Email) ? (LegalName ?? "?") : $"{LegalName ?? "?"} <{Email}>";
}

public class LarkEmployeeContractInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("employeeNumber")]
    public string? EmployeeNumber { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("effectiveDate")]
    public string? EffectiveDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("talentId")]
    public string? TalentId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("contractStartDate")]
    public string? ContractStartDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("contractEndDate")]
    public string? ContractEndDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("employmentType")]
    public string? EmploymentType { get; set; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
        => $"Number = {EmployeeNumber} & Status = {Status} & Type = {EmploymentType}";
}

public class LarkEmployeeJobInfo
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("jobId")]
    public string? JobId { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("employeeNumber")]
    public string? EmployeeNumber { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("level")]
    public LarkIdNameStaticInfo? Level { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("department")]
    public LarkIdNameStaticInfo? Department { get; set; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
        => $"Title = {Title ?? "?"} & Department = {Department?.Name ?? "?"} ({Department?.Id}) & Level = {Level?.Name ?? "?"} ({Level?.Id})";
}

public class LarkCompanyDepartmentInfo : IIdPropertyModel, INamePropertyModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("active")]
    public bool? IsActive { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("info")]
    public LarkCompanyDepartmentStaticInfo? Info { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("manager")]
    public LarkIdNameStaticInfo? Manager { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("parentDepartment")]
    public LarkIdNameStaticInfo? ParentDepartment { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("isRoot")]
    public bool? IsRoot { get; set; }

    [JsonIgnore]
    string? INamePropertyModel.Name => Info?.Name;

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
        => IsActive == false ? $"Inactived | {Info?.Name ?? "?"} ({Id})" : $"{Info?.Name ?? "?"} ({Id})";
}

public class LarkCompanyDepartmentStaticInfo : INamePropertyModel
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [JsonPropertyName("costCenterId")]
    public string? CostCenterId { get; set; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
        => $"Code = {Code} & Name = {Name}";
}
