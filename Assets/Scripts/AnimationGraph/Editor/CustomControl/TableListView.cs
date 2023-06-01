using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class TableListView : VisualElement
    {
        public class Column
        {
            public string name;
            public Func<VisualElement> title;
            public Func<VisualElement> cellTemplate;
            
            //Bind a cell in the specific row, to refresh VisualElement
            public Action<VisualElement, int> refreshCell;
        }
        
        public class Row
        {
            public VisualElement container;
            public List<VisualElement> cells = new List<VisualElement>();
        }
        
        private VisualElement m_TopContainer;
        private VisualElement m_TitleContainer;

        private List<Column> m_Columns = new List<Column>();
        private List<Row> m_Rows = new List<Row>();

        public Action<int> onAddRow;
        public Action<VisualElement, int> onDeleteRow;
        
        public TableListView()
        {
            style.height = 300;
            style.marginTop = style.marginLeft = style.marginRight = 10;
            style.borderTopWidth = style.borderBottomWidth =
                style.borderLeftWidth = style.borderRightWidth = 1;
            style.borderTopColor = style.borderBottomColor =
                style.borderLeftColor = style.borderRightColor = Color.gray;

            m_TopContainer = new VisualElement()
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.RowReverse),
                    height = 20,
                    borderBottomWidth = 1,
                    borderBottomColor = Color.gray
                }
            };

            m_TitleContainer = new VisualElement()
            {
                style =
                {
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row),
                    height = 20,
                    borderBottomWidth = 1,
                    borderBottomColor = Color.gray
                }
            };
            Add(m_TopContainer);
            Add(m_TitleContainer);

            Button addElementButton = new Button(AddRow);
            addElementButton.Add(new Label("+"));
            m_TopContainer.Add(addElementButton);

        }

        public void AddColumn(Column column)
        {
            m_Columns.Add(column);
            m_TitleContainer.Add(column.title.Invoke());
        }

        public void AddRow()
        {
            var row = new Row();

            var rowContainer = new VisualElement()
            {
                style =
                {
                    height = 20,
                    borderBottomWidth = 1,
                    borderBottomColor = Color.gray,
                    flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row)
                }
            };
            row.container = rowContainer;
            
            //Prepare row data first, then create row VisualElement
            onAddRow?.Invoke(m_Rows.Count);

            foreach (var column in m_Columns)
            {
                var cell = column.cellTemplate.Invoke();
                column.refreshCell.Invoke(cell, m_Rows.Count);
                rowContainer.Add(cell);
                row.cells.Add(cell);
            }
            
            var deleteButton = new Button(() =>
            {
                DeleteRow(row);
            });
            deleteButton.Add(new Label("×"));
            deleteButton.style.width = 20;
            rowContainer.Add(deleteButton);

            Add(rowContainer);
            m_Rows.Add(row);
        }

        private void DeleteRow(Row row)
        {
            //prepare data first
            onDeleteRow?.Invoke(row.container, m_Rows.IndexOf(row));
            
            Remove(row.container);
            m_Rows.Remove(row);
            
            foreach (var newRow in m_Rows)
            {
                for (int i = 0; i < m_Columns.Count; i++)
                {
                    m_Columns[i].refreshCell?.Invoke(newRow.cells[i], m_Rows.IndexOf(newRow));
                }
            }
        }
    }
}