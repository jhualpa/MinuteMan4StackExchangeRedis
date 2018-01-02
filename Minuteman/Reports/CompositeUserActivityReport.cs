using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Minuteman.Abstract;
using Minuteman.Helpers;

namespace Minuteman.Reports
{
    public class CompositeUserActivityReport : UserActivityReport
   {
       #region CONSTRUCTOR
       
       public CompositeUserActivityReport(
            IClient client,
            string reportkey,
            Func<IEnumerable<string>, Task<long>> operation,
            UserActivityReport left,
            UserActivityReport right)
            : base(client, reportkey)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            Operation = operation;
            Left = left;
            Right = right;
        }

       #endregion

       #region PROTECTED PROPERTIES

        protected Func<IEnumerable<string>, Task<long>> Operation { get; private set; }

        protected UserActivityReport Left { get; private set; }

        protected UserActivityReport Right { get; private set; }

       #endregion

        #region UserActivityReport MEMBERS

        public override async Task<bool[]> Includes(params long[] users)
        {
            Validation.ValidateUsers(users);

            await PerformBitOperation().ConfigureAwait(false);

            return await InternalIncludes(users).ConfigureAwait(false);
        }

        public override async Task<long> Count()
        {
            await PerformBitOperation().ConfigureAwait(false);

            return await InternalCount().ConfigureAwait(false);
            
        }

        #endregion

        #region INTERNAL METHODS

        internal async Task<long> PerformBitOperation()
        {
            var leftComposite = Left as CompositeUserActivityReport;
            var rightComposite = Right as CompositeUserActivityReport;

            var tasks = new List<Task>();

            if (leftComposite != null)
            {
                tasks.Add(leftComposite.PerformBitOperation());
            }

            if (rightComposite != null)
            {
                tasks.Add(rightComposite.PerformBitOperation());
            }

            if (tasks.Count > 0)
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }

            string[] keys = { Left.Key, Right.Key };

            return await Operation(keys).ConfigureAwait(false);
        }

        #endregion

   }
}