using System.ComponentModel.DataAnnotations;

namespace UnionWebApi.Application.Enums;
public enum ESort
{
    [Display(Name = "OrderBy")]
    ASC = 1,

    [Display(Name = "OrderByDescending")]
    DESC = 2
}
