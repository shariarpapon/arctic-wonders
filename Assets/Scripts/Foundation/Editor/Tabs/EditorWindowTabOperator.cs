using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Arctic.Foundation.Editor.Tabs
{
    public sealed class WindowTabOperator
    {
        internal sealed class Builder
        {
            private int m_defaultSelection;
            private readonly List<Tab> m_tabs;
            public Builder()
            {
                m_defaultSelection = 0;
                m_tabs = new List<Tab>();

            }

            public static Builder Init() 
            {
                return new Builder();
            }

            public WindowTabOperator Build()
            {
                WindowTabOperator opr = new WindowTabOperator();
                opr.m_tabs = m_tabs;
                opr.SetSelection(m_defaultSelection);
                return opr;
            }

            public Builder SetDefaultSelection(int index)
            {
                m_defaultSelection = index;
                return this;
            }

            public Builder RegisterTab(string name, System.Action drawCall)
            {
                return RegisterTabs(new Tab(name, drawCall));
            }

            public Builder RegisterTabs(params Tab[] tabs)
            {
                if (tabs == null || tabs.Length <= 0)
                {
                    Debug.LogWarning("Provided Tab array is either null or empty.");
                    return this;
                }

                foreach (Tab tab in tabs) 
                {
                    if (m_tabs.Find(c => c.Name == tab.Name) == null)
                        m_tabs.Add(tab);
                    else
                        Debug.LogWarning($"Tab with name '{tab.Name}' already exists. Only the first one will be registered.");
                }
                return this;
            }

            public Builder UnregisterTab(int index)
            {
                m_tabs.RemoveAt(index);
                return this;
            }
        }

        public event System.Action<int> OnSelected;
        public event System.Action<int> OnDeselected;

        private List<Tab> m_tabs;
        private int m_selection;

        public event System.Action<Tab> OnBeforeSelectedTabButtonRendered;
        public event System.Action<Tab> OnAfterSelectedTabButtonRendered;

        private WindowTabOperator() { }

        public void Operate()
        {
            OperateTabButtons();
            OperateSelectedTab();
        }

        public string GetTabName(int index) => m_tabs[index].Name;

        public bool TryGetTabIndex(string name, out int index) 
        {
            index = m_tabs.FindIndex(c => c.Name == name);
            return index >= 0;
        }

        public bool TrySetSelection(string tabName) 
        {
            for (int i = 0; i < m_tabs.Count; i++)
                if (m_tabs[i].Name == tabName) 
                {
                    SetSelection(i);
                    return true;
                }

            return false;
        }

        public bool SetSelection(int index)
        {
            if (index < 0 || index >= m_tabs.Count)
            {
                Debug.LogError($"Tab index out of bounds (index: {index})");
                return false;
            }

            if (m_selection != index)
            {
                int prevSelection = m_selection;
                m_selection = index;
                OnDeselected?.Invoke(prevSelection);
                OnSelected?.Invoke(index);
                return true;
            }

            return false;
        }

        private void OperateTabButtons() 
        {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < m_tabs.Count; i++)
            {
                if (m_tabs[i] == null) 
                {
                    Debug.LogError("Resgistered tab is null at index: " + i);
                    continue;
                }
                bool selectedTabButton = m_selection == i;
                if (selectedTabButton)
                    OnBeforeSelectedTabButtonRendered?.Invoke(m_tabs[i]);

                if (GUILayout.Button(m_tabs[i].Name))
                    SetSelection(i);

                if (selectedTabButton)
                    OnAfterSelectedTabButtonRendered?.Invoke(m_tabs[i]);
            }
            EditorGUILayout.EndHorizontal();

        }
        private void OperateSelectedTab()
        {
            m_tabs[m_selection].Draw.Invoke();
        }


    }
}