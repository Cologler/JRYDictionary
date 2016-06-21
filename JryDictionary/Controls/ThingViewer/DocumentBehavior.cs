using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;

namespace JryDictionary.Controls.ThingViewer
{
    public class DocumentBehavior : Behavior<TextBlock>
    {
        /// <summary>
        /// ����Ϊ���ӵ� AssociatedObject ����á�
        /// </summary>
        /// <remarks>
        /// ������Ա㽫���ܹҹ��� AssociatedObject��
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.RebuildInlines();
            this.AssociatedObject.DataContextChanged += this.AssociatedObject_DataContextChanged;
        }

        private void RebuildInlines()
        {
            if (this.AssociatedObject == null) return;
            var vm = this.AssociatedObject.DataContext as ThingViewerViewModel;
            this.AssociatedObject.Inlines.Clear();
            if (vm?.Description != null)
            {
                var inlines = vm.BuildDescriptionInlines().ToArray();
                this.AssociatedObject.Inlines.AddRange(inlines);
                foreach (var link in inlines.OfType<Hyperlink>())
                {
                    link.RequestNavigate += this.Link_RequestNavigate;
                }
            }
        }

        private void Link_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Debug.WriteLine(e.Uri.AbsoluteUri);
        }

        private void AssociatedObject_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) => this.RebuildInlines();

        /// <summary>
        /// ����Ϊ���� AssociatedObject ����ʱ��������ʵ�ʷ���֮ǰ�����á�
        /// </summary>
        /// <remarks>
        /// ������Ա㽫���ܴ� AssociatedObject �н���ҹ���
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.DataContextChanged -= this.AssociatedObject_DataContextChanged;
        }
    }
}