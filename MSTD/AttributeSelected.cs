using System;
using System.Collections.Generic;
using MSTD.ShBase;

namespace CFL_1.CFL_System.MSTD
{
    public delegate IEnumerable<object> SelectableDataSource();

    public class AttributeSelected : Attribute
    {
        public AttributeSelected(SelectableDataSource data)
        {
            SelectableDataSource = data;
        }

        private SelectableDataSource SelectableDataSource { get; set; }

        public IEnumerable<object> DataSource()
        {
            if(SelectableDataSource != null)
            {
                foreach(object _o in SelectableDataSource.Invoke())
                    yield return _o;
            }
        }
    }
}
