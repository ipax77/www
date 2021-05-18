using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIKit;
using WebKit;
using WorldWideWalk.iOS;
using WorldWideWalk.Models;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(MapWebView), typeof(MapWebViewRenderer))]
namespace WorldWideWalk.iOS
{
    class MapWebViewRenderer : WkWebViewRenderer
    {
        public MapWebViewRenderer() : this(new WKWebViewConfiguration())
        {
        }

        public MapWebViewRenderer(WKWebViewConfiguration config) : base(config)
        {
        }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
        }

        //public override bool DispatchTouchEvent(MotionEvent e)
        //{
        //    Parent.RequestDisallowInterceptTouchEvent(true);
        //    return base.DispatchTouchEvent(e);
        //}
    }
}