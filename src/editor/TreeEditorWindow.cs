﻿#region License

/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2014 Vadim Macagon
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

#endregion License

using UnityEditor;
using UnityEngine;

namespace UBonsai.Editor
{
    public class TreeEditorWindow : EditorWindow
    {
        public const int InvalidWindowID = -1;

        private Tree _currentTree = new Tree();
        private Vector2 _mousePosition;
        private bool _dragging = false;

        private static int _nextWindowID = 0;

        [MenuItem("Window/UBonsai Editor")]
        private static void Init()
        {
            EditorWindow.GetWindow<TreeEditorWindow>("UBonsai");
        }

        /// <summary>
        /// Generate a unique window ID for a GUI.Window.
        /// The generated ID is only unique for any instances of TreeEditorWindow.
        /// </summary>
        /// <returns>A unique ID that can be used to create a new GUI.Window.</returns>
        public static int GenerateWindowID()
        {
            return _nextWindowID++;
        }

        /// <summary>
        /// Handle input and redraw the window if necessary.
        /// </summary>
        public void OnGUI()
        {
            Event e = Event.current;
            _mousePosition = e.mousePosition;

            GUI.changed = false;

            switch (e.type)
            {
                case EventType.MouseDrag:
                    // to achieve smooth dragging wantsMouseMove must be set,
                    // doing so will prompt Unity to call OnGUI() more frequently
                    if (!_dragging)
                    {
                        _dragging = true;
                        wantsMouseMove = true;
                    }
                    // note that the event is not marked as used at this point because
                    // the actual dragging is handled further down by the contents of
                    // the editor window
                    break;

                case EventType.MouseUp:
                    _dragging = false;
                    wantsMouseMove = false;
                    break;
            }

            GUI.skin = GUISkinManager.Skin;
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            // need this in order for the node windows to show up
            BeginWindows();

            if (_currentTree != null)
                _currentTree.OnGUI(e);

            EndWindows();
            GUILayout.EndArea();

            // handle any events that haven't been used up above
            switch (e.type)
            {
                case EventType.ContextClick:
                    OnContextClick();
                    e.Use();
                    break;
            }

            if (GUI.changed || _currentTree.Dirty)
                Repaint();
        }

        private void OnContextClick()
        {
        }
    }
} // namespace UBonsai