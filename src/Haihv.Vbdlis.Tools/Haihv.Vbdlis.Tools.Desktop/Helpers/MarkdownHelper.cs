using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Avalonia.Controls.Documents;

namespace Haihv.Vbdlis.Tools.Desktop.Helpers
{
    /// <summary>
    /// Helper class for parsing simple markdown to Avalonia inlines
    /// </summary>
    public static class MarkdownHelper
    {
        /// <summary>
        /// Parses simple markdown text to Avalonia inlines
        /// Supports: headings (###), bullets (- or *), bold (**text**)
        /// </summary>
        public static IEnumerable<Inline> ParseToInlines(string input)
        {
            // Minimal markdown-like parser for update notes:
            // - Headings: ### Title
            // - Bullets: - item
            // - Bold: **text**
            // - Blank lines
            if (string.IsNullOrEmpty(input))
            {
                yield break;
            }

            var normalized = input.Replace("\r\n", "\n").Replace("\r", "\n");
            var lines = normalized.Split('\n');

            var firstLine = true;
            foreach (var raw in lines)
            {
                var line = (raw ?? string.Empty).TrimEnd();

                if (!firstLine)
                {
                    yield return new LineBreak();
                }

                firstLine = false;

                if (string.IsNullOrWhiteSpace(line))
                {
                    // Keep a blank line for readability
                    yield return new LineBreak();
                    continue;
                }

                // Heading (e.g. ### ...)
                if (Regex.IsMatch(line, @"^#{1,6}\s+"))
                {
                    var headingText = Regex.Replace(line, @"^#{1,6}\s+", string.Empty).Trim();
                    var headingSpan = new Span
                    {
                        FontWeight = Avalonia.Media.FontWeight.SemiBold,
                        FontSize = 13
                    };
                    foreach (var i in ParseBoldSegments(headingText))
                        headingSpan.Inlines!.Add(i);
                    yield return headingSpan;
                    continue;
                }

                // Bullet (- item)
                if (Regex.IsMatch(line, @"^[-*]\s+"))
                {
                    yield return new Run("• ");
                    var bulletText = Regex.Replace(line, @"^[-*]\s+", string.Empty);
                    foreach (var i in ParseBoldSegments(bulletText))
                        yield return i;
                    continue;
                }

                // Plain line (may contain **bold**)
                foreach (var i in ParseBoldSegments(line))
                    yield return i;
            }
        }

        /// <summary>
        /// Parses bold segments (**text**) from a line
        /// </summary>
        private static IEnumerable<Inline> ParseBoldSegments(string line)
        {
            // Split by **bold** segments. This is intentionally simple and safe.
            // Example: "abc **bold** def" -> Run("abc ") + Bold(Run("bold")) + Run(" def")
            if (string.IsNullOrEmpty(line))
                yield break;

            var parts = Regex.Split(line, @"(\*\*[^*]+\*\*)");
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part))
                    continue;

                if (part.StartsWith("**") && part.EndsWith("**") && part.Length >= 4)
                {
                    var boldText = part.Substring(2, part.Length - 4);
                    var bold = new Bold();
                    bold.Inlines!.Add(new Run(boldText));
                    yield return bold;
                }
                else
                {
                    yield return new Run(part);
                }
            }
        }

        /// <summary>
        /// Extracts app update notes from full release body using markers
        /// </summary>
        public static string ExtractAppUpdateNotes(string fullReleaseNotes)
        {
            const string startMarker = "<!-- APP_UPDATE_NOTES_START -->";
            const string endMarker = "<!-- APP_UPDATE_NOTES_END -->";

            if (string.IsNullOrWhiteSpace(fullReleaseNotes))
            {
                return string.Empty;
            }

            var startIndex = fullReleaseNotes.IndexOf(startMarker, StringComparison.Ordinal);
            var endIndex = fullReleaseNotes.IndexOf(endMarker, StringComparison.Ordinal);

            if (startIndex >= 0 && endIndex > startIndex)
            {
                startIndex += startMarker.Length;
                return fullReleaseNotes.Substring(startIndex, endIndex - startIndex).Trim();
            }

            // If markers are missing, fall back to the full body.
            return fullReleaseNotes.Trim();
        }
    }
}