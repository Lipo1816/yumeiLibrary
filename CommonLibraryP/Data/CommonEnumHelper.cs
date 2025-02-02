using DevExpress.Blazor;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibraryP.Data
{
    public static class CommonEnumHelper
    {
        public static IEnumerable<StatusStyle> StatusStyles =>
            new List<StatusStyle>
            {
                new (Status.Init, ButtonRenderStyle.Secondary, Color.FromArgb(143, 143, 143)),
                new (Status.TryConnecting, ButtonRenderStyle.Info, Color.FromArgb(91, 91, 174)),
                new (Status.Disconnect, ButtonRenderStyle.Danger, Color.FromArgb(204, 0, 0)),
                new (Status.Idle, ButtonRenderStyle.Info, Color.FromArgb(130, 192, 192)),
                new (Status.Running, ButtonRenderStyle.Success, Color.FromArgb(1, 178, 104)),
                new (Status.Pause, ButtonRenderStyle.Warning, Color.FromArgb(235, 192, 0)),
                new (Status.Stop, ButtonRenderStyle.Danger, Color.FromArgb(204, 0, 0)),
                new (Status.Error, ButtonRenderStyle.Danger, Color.FromArgb(204, 0, 0)),
            };

        public static StatusStyle? GetStatusStyle(int statusCode)
        {
            var target = StatusStyles.FirstOrDefault(x => (int)x.status == statusCode);
            return target;
        }
    }
    public class StatusWrapperClass : EnumWrapper
    {
        public StatusWrapperClass(Status status)
        {
            Status = status;
            index = (int)status;
            displayName = status.ToString();
        }

        public Status Status { get; init; }
    }
    public class StatusStyle
    {
        public Status status { get; init; }
        public ButtonRenderStyle buttonRenderStyle { get; init; }
        public Color StyleColor { get; init; }
        public string ColorRGBString => $"RGB({StyleColor.R}, {StyleColor.G}, {StyleColor.B})";
        public string ColorHTMLString => $"#{StyleColor.R:X2}{StyleColor.G:X2}{StyleColor.B:X2}";
        public StatusStyle(Status status, ButtonRenderStyle buttonRenderStyle, Color color)
        {
            this.status = status;
            this.buttonRenderStyle = buttonRenderStyle;
            this.StyleColor = color;
        }
    }
    public enum Status
    {
        Init,
        TryConnecting,
        Disconnect,

        Idle,
        Running,
        Pause,
        Stop,
        Error,
    }
}
