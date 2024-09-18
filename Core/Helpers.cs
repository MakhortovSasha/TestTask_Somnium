using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.IO;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Runtime.CompilerServices;
using System.Windows.Media.Animation;

namespace TestTask_Somnium.Core.Helpers
{
    
    public static class CoreHelper
    {
        public static bool TryGetChildOfType<T>(this DependencyObject depObj, ref T result)where T : DependencyObject
        {

            if (depObj == null)
            {
                return false;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                result = (child as T);
                if (result == null)
                 if (TryGetChildOfType<T>(child, ref result)) return true; 
                else continue;
                return true;

            }
            return false;
        }

        public static string RemoveXMLMarkup(this string before)
        {
            return before
                .Replace(" PagePadding=\"5,0,5,0\" AllowDrop=\"True\"", string.Empty)
                .Replace("<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">", string.Empty)
                .Replace("<FlowDocument xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" />", string.Empty)
                .Replace("</FlowDocument>", string.Empty)
                .Replace("<Paragraph xml:space=\"preserve\">", string.Empty)
                .Replace("</Paragraph>", string.Empty)
                .Replace("<Paragraph xml:space=\"preserve\" />", string.Empty);

        }
    }

    public class RichTextBoxHelper : DependencyObject
    {
        private static HashSet<Thread> _recursionProtection = new HashSet<Thread>();

        public static string GetDocumentXaml(DependencyObject obj)
        {
            return (string)obj.GetValue(DocumentXamlProperty);
        }

        public static void SetDocumentXaml(DependencyObject obj, string value)
        {
            _recursionProtection.Add(Thread.CurrentThread);
            obj.SetValue(DocumentXamlProperty, value);
            _recursionProtection.Remove(Thread.CurrentThread);
        }

        public static readonly DependencyProperty DocumentXamlProperty = DependencyProperty.RegisterAttached(
            "DocumentXaml",
            typeof(string),
            typeof(RichTextBoxHelper),
            new FrameworkPropertyMetadata(
                "",
                FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                (obj, e) => {
                    if (_recursionProtection.Contains(Thread.CurrentThread))
                        return;

                    var richTextBox = (RichTextBox)obj;

                    // Parse the XAML to a document (or use XamlReader.Parse())

                    try
                    {
                        var stream = new MemoryStream(Encoding.UTF8.GetBytes(GetDocumentXaml(richTextBox)));
                        var doc = (FlowDocument)XamlReader.Load(stream);

                        // Set the document
                        richTextBox.Document = doc;
                    }
                    catch (Exception)
                    {
                        richTextBox.Document = new FlowDocument();
                    }

                    // When the document changes update the source
                    richTextBox.TextChanged += (obj2, e2) =>
                    {
                        RichTextBox richTextBox2 = obj2 as RichTextBox;
                        if (richTextBox2 != null)
                        {
                            SetDocumentXaml(richTextBox, XamlWriter.Save(richTextBox2.Document));
                        }
                    };
                }
            )
        );
    }

}
