using System;
using UnityEngine;

namespace GuruLaghima
{
  //
  // Summary:
  //     Specify a tooltip for a field in the Inspector window.
  [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
  public class CustomTooltipAttribute : PropertyAttribute
  {
    //
    // Summary:
    //     The tooltip text.
    public readonly string tooltip;

    //
    // Summary:
    //     Specify a tooltip for a field.
    //
    // Parameters:
    //   tooltip:
    //     The tooltip text.
    public CustomTooltipAttribute(string tooltip)
    {
      this.tooltip = tooltip;
    }
  }
}