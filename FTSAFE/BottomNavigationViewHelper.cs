using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Internal;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace FTSAFE
{
    class BottomNavigationViewHelper
    {
        public static void disableShiftMode(BottomNavigationView view)
        {
            BottomNavigationMenuView menuView = (BottomNavigationMenuView)view.GetChildAt(0);
            try
            {
                /*
                 * Field              -----------   在Java反射中 Field类描述的是 类的属性信息,通俗来讲 有一个类如下：
                 * getClass()         -----------   返回Class类型的对象
                 * getDeclaredField() -----------   获取类本身的属性成员(包括私有、共有、保护） 
                 * setAccessible()    -----------   访问私有属性
                 * setBoolean()       -----------   将字段的值设置为指定对象上的布尔值
                 * 
                */
                /*C# 反射类*/
                var shiftMode = menuView.Class.GetDeclaredField("mShiftingMode");
                shiftMode.Accessible = true;
                shiftMode.SetBoolean(menuView, false);
                shiftMode.Accessible = false;
                shiftMode.Dispose();
                for (var i = 0; i < menuView.ChildCount; i++)
                {
                    var item = menuView.GetChildAt(i) as BottomNavigationItemView;
                    if (item == null) continue;
                    item.SetShiftingMode(false);
                }
                if (menuView.ChildCount > 0)
                    menuView.UpdateMenuView(); 
            }
            catch (NoSuchFieldException e)
            {

            }
            catch (IllegalAccessException e)
            {

            }
        }
    }
}