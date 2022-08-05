using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using KitX_Dashboard.ViewModels.Pages.Controls;
using System.Collections.Generic;

namespace KitX_Dashboard.Views.Pages.Controls
{
    public partial class Settings_About : UserControl
    {
        private readonly Settings_AboutViewModel viewModel = new();

        public Settings_About()
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private readonly List<Key> keyqueue = new();

        private readonly List<Key> rightkey = new()
        {
            Key.W, Key.W, Key.S, Key.S, Key.A, Key.D, Key.A, Key.D, Key.B, Key.A
        };

        private void AppNameButtonKeyDown(object sender, KeyEventArgs e)
        {
            if (viewModel.clickCount >= 7)
            {
                if (keyqueue.Count >= 10)
                    keyqueue.RemoveAt(0);
                keyqueue.Add(e.Key);

                if (keyqueue.Count == 10)
                {
                    bool pass = true;
                    for (int i = 0; i < keyqueue.Count; ++i)
                        if (keyqueue[i] != (rightkey[i]))
                        {
                            pass = false;
                            break;
                        }
                    viewModel.AuthorsListVisibility = pass;
                }
            }
        }
    }
}

//
//                       d*##$.
//  zP"""""$e.           $"    $o
// 4$       '$          $"      $
// '$        '$        J$       $F
//  'b        $k       $&gt;       $
//   $k        $r     J$       d$
//   '$         $     $"       $~
//    '$        "$   '$E       $
//     $         $L   $"      $F ...
//      $.       4B   $      $$$*"""*b
//      '$        $.  $$     $$      $F
//       "$       R$  $F     $"      $
//        $k      ?$ u*     dF      .$
//        ^$.      $$"     z$      u$$$$e
//         #$b             $E.dW@e$"    ?$
//          #$           .o$$# d$$$$c    ?F
//           $      .d$$#" . zo$&gt;   #$r .uF
//           $L .u$*"      $&amp;$$$k   .$$d$$F
//            $$"            ""^"$$$P"$P9$
//           JP              .o$$$$u:$P $$
//           $          ..ue$"      ""  $"
//          d$          $F              $
//          $$     ....udE             4B
//           #$    """"` $r            @$
//            ^$L        '$            $F
//              RN        4N           $
//               *$b                  d$
//                $$k                 $F
//                $$b                $F
//                  $""               $F
//                  '$                $
//                   $L               $
//                   '$               $
//                    $               $
//
