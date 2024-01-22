using System;
using System.Threading.Tasks;

namespace Corelibs.Basic.Blocks
{
    /// <summary>
    /// Functional data data to represent a discriminated
    /// union of two possible types.
    /// </summary>
    /// <typeparam name="TL">Type of "Left" item.</typeparam>
    /// <typeparam name="TR">Type of "Right" item.</typeparam>
    public class Either<TL, TR>
    {
        private readonly TL _left;
        private readonly TR _right;
        private readonly bool _isLeft;

        public static Either<TL, TR> Create(TL left)
        {
            return new Either<TL, TR>(left);
        }

        public static Either<TL, TR> Create(TR right)
        {
            return new Either<TL, TR>(right);
        }

        public static Task<Either<TL, TR>> CreateTask(TL left)
        {
            return Task.FromResult(new Either<TL, TR>(left));
        }

        public static Task<Either<TL, TR>> CreateTask(TR right)
        {
            return Task.FromResult(new Either<TL, TR>(right));
        }

        public Either(TL left)
        {
            _left = left;
            _isLeft = true;
        }

        public Either(TR right)
        {
            _right = right;
            _isLeft = false;
        }

        /// <summary>
        /// If right value is assigned, execute an action on it.
        /// </summary>
        /// <param name="rightAction">Action to execute.</param>
        public void DoRight(Action<TR> rightAction)
        {
            if (rightAction == null)
                throw new ArgumentNullException(nameof(rightAction));

            if (IsRight)
                rightAction(_right);
        }

        /// <summary>
        /// If left value is assigned, execute an action on it.
        /// </summary>
        /// <param name="rightAction">Action to execute.</param>
        public void DoLeft(Action<TL> leftAction)
        {
            if (leftAction == null)
                throw new ArgumentNullException(nameof(leftAction));

            if (IsLeft)
                leftAction(_left);
        }

        public bool IsLeft => _isLeft;
        public bool IsRight => !_isLeft;


        public static implicit operator Either<TL, TR>(TL left) => new Either<TL, TR>(left);

        public static implicit operator Either<TL, TR>(TR right) => new Either<TL, TR>(right);
    }
}
