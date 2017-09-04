using NodeInspector;

namespace PaintCraft.Tools{
    public abstract class FilterWithNextNode : IFilter {
        [OneWay]
        public IFilter NextFilter;

        #region implemented abstract members of FilterWithNextNode
        public override void Apply(BrushContext brushLineContext)
        {
            if (brushLineContext.Points.Count == 0){
                UnityEngine.Debug.Log("Im here");
            }

            bool runNextFilter = FilterBody(brushLineContext);

            if (runNextFilter){                
                RunNextFilter(brushLineContext);
            }
               
            FilterFinalizer(brushLineContext);
        }
        #endregion


        /// <summary>
        /// Filters the body.
        /// </summary>
        /// <returns><c>true</c>, if you want to run nextFilter, <c>false</c> otherwise.</returns>
        /// <param name="brushLineContext">Brush line context.</param>
        public abstract bool FilterBody(BrushContext brushLineContext);


        /// <summary>
        /// Filters the finalizer. 
        /// We run this finalizer after nextFilter execution done
        /// </summary>
        /// <param name="brushLineContext">Brush line context.</param>
        protected virtual void FilterFinalizer(BrushContext brushLineContext){
            
        }


        public void RunNextFilter(BrushContext brushContext){
            if (NextFilter != null){
                NextFilter.Apply(brushContext);
            }
        }
    }       
}