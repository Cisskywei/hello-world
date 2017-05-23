using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace veClassRoom.Room
{
    class ModelManager
    {
        public static ModelManager getInstance()
        {
            return Singleton<ModelManager>.getInstance();
        }

        public Dictionary<string, IndependentMode> independent = new Dictionary<string, IndependentMode>();
        Queue<IndependentMode> independentunuse = new Queue<IndependentMode>();
        public Dictionary<string, WatchAndLearnModel> watchandlearn = new Dictionary<string, WatchAndLearnModel>();
        Queue<WatchAndLearnModel> watchandlearnunuse = new Queue<WatchAndLearnModel>();
        public Dictionary<string, GuidanceMode> guidance = new Dictionary<string, GuidanceMode>();
        Queue<GuidanceMode> guidanceunuse = new Queue<GuidanceMode>();
        public Dictionary<string, CollaborativeModel> collaborative = new Dictionary<string, CollaborativeModel>();
        Queue<CollaborativeModel> collaborativeunuse = new Queue<CollaborativeModel>();

        public BaseModelClass ApplyModelCtrl(Enums.ModelEnums model, string scenename = "TestScene")
        {
            BaseModelClass retmodel = null;

            switch (model)
            {
                case Enums.ModelEnums.Separate:
                    //if(independentunuse.Count > 0)
                    //{
                    //    retmodel = independentunuse.Dequeue();
                    //}
                    //else
                    //{
                    //    retmodel = new IndependentMode();
                    //    if(!independent.ContainsKey(scenename))
                    //    {
                    //        independent.Add(scenename, retmodel as IndependentMode);
                    //    }
                    //}
                    retmodel = new IndependentMode();
                    break;
                case Enums.ModelEnums.SynchronousOne:
                    //if (watchandlearnunuse.Count > 0)
                    //{
                    //    retmodel = watchandlearnunuse.Dequeue();
                    //}
                    //else
                    //{
                    //    retmodel = new WatchAndLearnModel();
                    //    if (!watchandlearn.ContainsKey(scenename))
                    //    {
                    //        watchandlearn.Add(scenename, retmodel as WatchAndLearnModel);
                    //    }
                    //}
                    retmodel = new WatchAndLearnModel();
                    break;
                case Enums.ModelEnums.SynchronousMultiple:
                    //if (guidanceunuse.Count > 0)
                    //{
                    //    retmodel = guidanceunuse.Dequeue();
                    //}
                    //else
                    //{
                    //    retmodel = new GuidanceMode();
                    //    if (!guidance.ContainsKey(scenename))
                    //    {
                    //        guidance.Add(scenename, retmodel as GuidanceMode);
                    //    }
                    //}
                    retmodel = new GuidanceMode();
                    break;
                case Enums.ModelEnums.Collaboration:
                    //if (collaborativeunuse.Count > 0)
                    //{
                    //    retmodel = collaborativeunuse.Dequeue();
                    //}
                    //else
                    //{
                    //    retmodel = new CollaborativeModel();
                    //    if (!collaborative.ContainsKey(scenename))
                    //    {
                    //        collaborative.Add(scenename, retmodel as CollaborativeModel);
                    //    }
                    //}
                    retmodel = new CollaborativeModel();
                    break;
                default:
                    break;
            }

            return retmodel;
        }

        public void ReleaseModelBySceneName(Enums.ModelEnums model, string scenename = "TestScene")
        {
            //switch (model)
            //{
            //    case Enums.ModelEnums.Separate:
            //        if(independent.ContainsKey(scenename))
            //        {
            //            independentunuse.Enqueue(independent[scenename]);
            //            independent.Remove(scenename);
            //        }
            //        break;
            //    case Enums.ModelEnums.SynchronousOne:
            //        if (watchandlearn.ContainsKey(scenename))
            //        {
            //            watchandlearnunuse.Enqueue(watchandlearn[scenename]);
            //            watchandlearn.Remove(scenename);
            //        }
            //        break;
            //    case Enums.ModelEnums.SynchronousMultiple:
            //        if (guidance.ContainsKey(scenename))
            //        {
            //            guidanceunuse.Enqueue(guidance[scenename]);
            //            guidance.Remove(scenename);
            //        }
            //        break;
            //    case Enums.ModelEnums.Collaboration:
            //        if (collaborative.ContainsKey(scenename))
            //        {
            //            collaborativeunuse.Enqueue(collaborative[scenename]);
            //            collaborative.Remove(scenename);
            //        }
            //        break;
            //    default:
            //        break;
            //}
        }
    }
}
