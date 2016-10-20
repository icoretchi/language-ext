﻿using LanguageExt.TypeClasses;
using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Add the bound value of Try(x) to Try(y).  If either of the
        /// Trys are Fail then the result is Fail
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public static Try<A> add<ADD, A>(Try<A> lhs, Try<A> rhs) where ADD : struct, Addition<A> =>
            lhs.Add<ADD, A>(rhs);
    
        /// <summary>
        /// Subtract the Try(x) from Try(y).  If either of the Trys throw then the result is Fail
        /// For numeric values the behaviour is to find the difference between the Trys (lhs - rhs)
        /// For Lst values the behaviour is to remove items in the rhs from the lhs
        /// For Map or Set values the behaviour is to remove items in the rhs from the lhs
        /// Otherwise if the R type derives from ISubtractable then the behaviour
        /// is to call lhs.Subtract(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public static Try<T> difference<SUB, T>(Try<T> lhs, Try<T> rhs) where SUB : struct, Difference<T> =>
            lhs.Difference<SUB, T>(rhs);

        /// <summary>
        /// Find the product of Try(x) and Try(y).  If either of the Trys throw then the result is Fail
        /// For numeric values the behaviour is to multiply the Trys (lhs * rhs)
        /// For Lst values the behaviour is to multiply all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IMultiplicable then the behaviour
        /// is to call lhs.Multiply(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        [Pure]
        public static Try<T> product<PROD, T>(Try<T> lhs, Try<T> rhs) where PROD : struct, Product<T> =>
            lhs.Product<PROD, T>(rhs);

        /// <summary>
        /// Divide Try(x) by Try(y).  If either of the Trys throw then the result is Fail
        /// For numeric values the behaviour is to divide the Trys (lhs / rhs)
        /// For Lst values the behaviour is to divide all combinations of values in both lists 
        /// to produce a new list
        /// Otherwise if the R type derives from IDivisible then the behaviour
        /// is to call lhs.Divide(rhs);
        /// </summary>
        /// <param name="lhs">Left-hand side of the operation</param>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        [Pure]
        public static Try<T> divide<DIV, T>(Try<T> lhs, Try<T> rhs) where DIV : struct, Divisible<T> =>
            lhs.Divide<DIV, T>(rhs);

        /// <summary>
        /// Apply a Try argument to a Try function of arity 1
        /// </summary>
        /// <param name="self">Try function</param>
        /// <param name="arg">Try argument</param>
        /// <returns>Returns the result of applying the Try argument to the Try function</returns>
        [Pure]
        public static Try<R> apply<T, R>(Try<Func<T, R>> self, Try<T> arg) =>
            self.Apply<Try<R>, T, R>(arg);

        /// <summary>
        /// Apply Try arguments to a Try function of arity 2
        /// </summary>
        /// <param name="self">Try function</param>
        /// <param name="arg">Try argument</param>
        /// <returns>Returns the result of applying the Try argument to the Try function</returns>
        [Pure]
        public static Try<R> apply<T, U, R>(Try<Func<T, U, R>> self, Try<T> arg1, Try<U> arg2) =>
            self.Apply<Try<R>, T, U, R>(arg1, arg2);

        /// <summary>
        /// Partially apply a Try argument to a curried Try function
        /// </summary>
        [Pure]
        public static Try<Func<U, R>> apply<T, U, R>(Try<Func<T, Func<U, R>>> self, Try<T> arg) =>
            self.Apply<Try<Func<U, R>>, T, U, R>(arg);

        /// <summary>
        /// Partially apply Try values to a Try function of arity 2
        /// </summary>
        /// <param name="self">Try function</param>
        /// <param name="arg1">Try argument</param>
        /// <returns>Returns the result of applying the Try arguments to the Try function</returns>
        [Pure]
        public static Try<Func<T2, R>> apply<T1, T2, R>(Try<Func<T1, T2, R>> self, Try<T1> arg1) =>
            self.Apply<Try<Func<T2, R>>, T1, T2, R>(arg1);

        /// <summary>
        /// Test if the Try computation is successful
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation to test</param>
        /// <returns>True if successful</returns>
        [Pure]
        public static bool isSucc<T>(Try<T> self) =>
            !isFail(self);

        /// <summary>
        /// Test if the Try computation fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation to test</param>
        /// <returns>True if fails</returns>
        [Pure]
        public static bool isFail<T>(Try<T> self) =>
            self.Try().IsFaulted;

        /// <summary>
        /// Invoke a delegate if the Try returns a value successfully
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Succ">Delegate to invoke if successful</param>
        public static Unit ifSucc<T>(Try<T> self, Action<T> Succ) =>
            self.IfSucc(Succ);

        /// <summary>
        /// Invoke a delegate if the Try fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="value">Try computation</param>
        /// <param name="Fail">Delegate to invoke on failure</param>
        /// <returns>Result of the invocation of Fail on failure, the result of the Try otherwise</returns>
        [Pure]
        public static T ifFail<T>(Try<T> self, Func<T> Fail) =>
            self.IfFail(Fail);

        /// <summary>
        /// Return a default value if the Try fails
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="failValue">Default value to use on failure</param>
        /// <returns>failValue on failure, the result of the Try otherwise</returns>
        [Pure]
        public static T ifFail<T>(Try<T> self, T failValue) =>
            self.IfFail(failValue);

        /// <summary>
        /// Provides a fluent exception matching interface which is invoked
        /// when the Try fails.
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Fluent exception matcher</returns>
        [Pure]
        public static ExceptionMatch<T> ifFail<T>(Try<T> self) =>
            self.IfFail();

        /// <summary>
        /// Maps the bound value to the resulting exception (if the Try computation fails).  
        /// If the Try computation succeeds then a NotSupportedException is used.
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Try of Exception</returns>
        [Pure]
        public static Try<Exception> failed<T>(Try<T> self) =>
            bimap(self, 
                Succ: _  => new NotSupportedException(),
                Fail: ex => ex
                );

        /// <summary>
        /// Flattens nested Try computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Flattened Try computation</returns>
        [Pure]
        public static Try<T> flatten<T>(Try<Try<T>> self) =>
            self.Flatten();

        /// <summary>
        /// Flattens nested Try computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Flattened Try computation</returns>
        [Pure]
        public static Try<T> flatten<T>(Try<Try<Try<T>>> self) =>
            self.Flatten();

        /// <summary>
        /// Flattens nested Try computations
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <returns>Flattened Try computation</returns>
        [Pure]
        public static Try<T> flatten<T>(Try<Try<Try<Try<T>>>> self) =>
            self.Flatten();

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        /// <returns>The result of either the Succ or Fail delegates</returns>
        [Pure]
        public static R match<T, R>(Try<T> self, Func<T, R> Succ, Func<Exception, R> Fail) =>
            self.Match(Succ, Fail);

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Type of the resulting bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Default value to use if the Try computation fails</param>
        /// <returns>The result of either the Succ delegate or the Fail value</returns>
        [Pure]
        public static R match<T, R>(Try<T> self, Func<T, R> Succ, R Fail) =>
            self.Match(Succ, Fail);

        /// <summary>
        /// Pattern matches the two possible states of the Try computation
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Succ">Delegate to invoke if the Try computation completes successfully</param>
        /// <param name="Fail">Delegate to invoke if the Try computation fails</param>
        public static Unit match<T>(Try<T> self, Action<T> Succ, Action<Exception> Fail) =>
            self.Match(Succ, Fail);

        /// <summary>
        /// Invokes a delegate with the result of the Try computation if it is successful.
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="action">Delegate to invoke on successful invocation of the Try computation</param>
        public static Unit iter<T>(Try<T> self, Action<T> action) =>
            self.Iter(action);

        /// <summary>
        /// Folds the value of Try into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="self">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, T>(Try<T> self, S state, Func<S, T, S> folder) =>
            self.Fold(state, folder);

        /// <summary>
        /// Folds the result of Try into an S.
        /// https://en.wikipedia.org/wiki/Fold_(higher-order_function)
        /// </summary>
        /// <param name="tryDel">Try to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="Succ">Fold function when Try succeeds</param>
        /// <param name="Fail">Fold function when Try fails</param>
        /// <returns>Folded state</returns>
        [Pure]
        public static S fold<S, T>(Try<T> self, S state, Func<S, T, S> Succ, Func<S, Exception, S> Fail) =>
            self.BiFold(state, Succ, Fail);

        /// <summary>
        /// Tests that a predicate holds for all values of the bound value T
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="pred">Predicate to test the bound value against</param>
        /// <returns>True if the predicate holds for the bound value, or if the Try computation
        /// fails.  False otherwise.</returns>
        [Pure]
        public static bool forall<T>(Try<T> self, Func<T, bool> pred) =>
            self.ForAll(pred);

        /// <summary>
        /// Counts the number of bound values.  
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">TrTry computation</param>
        /// <returns>1 if the Try computation is successful, 0 otherwise.</returns>
        [Pure]
        public static int count<T>(Try<T> self) =>
            self.Count();

        /// <summary>
        /// Tests that a predicate holds for any value of the bound value T
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="pred">Predicate to test the bound value against</param>
        /// <returns>True if the predicate holds for the bound value.  False otherwise.</returns>
        [Pure]
        public static bool exists<T>(Try<T> self, Func<T, bool> pred) =>
            self.Exists(pred);

        /// <summary>
        /// Maps the bound value
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Resulting bound value type</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="mapper">Delegate to map the bound value</param>
        /// <returns>Mapped Try computation</returns>
        [Pure]
        public static Try<R> map<T, R>(Try<T> self, Func<T, R> mapper) =>
            self.Map(mapper);

        /// <summary>
        /// Maps the bound value
        /// </summary>
        /// <typeparam name="T">Type of the bound value</typeparam>
        /// <typeparam name="R">Resulting bound value type</typeparam>
        /// <param name="self">Try computation</param>
        /// <param name="Succ">Delegate to map the bound value</param>
        /// <param name="Fail">Delegate to map the exception to the desired bound result type</param>
        /// <returns>Mapped Try computation</returns>
        [Pure]
        public static Try<R> bimap<T, R>(Try<T> tryDel, Func<T, R> Succ, Func<Exception, R> Fail) =>
            tryDel.BiMap(Succ, Fail);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static Try<Func<T2, R>> parmap<T1, T2, R>(Try<T1> self, Func<T1, T2, R> func) =>
            self.ParMap(func);

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public static Try<Func<T2, Func<T3, R>>> parmap<T1, T2, T3, R>(Try<T1> self, Func<T1, T2, T3, R> func) =>
            self.ParMap(func);

        [Pure]
        public static Try<T> filter<T>(Try<T> self, Func<T, bool> pred) =>
            self.Filter(pred);

        [Obsolete]
        public static Try<T> filter<T>(Try<T> self, Func<T, bool> Succ, Func<Exception, bool> Fail) =>
            self.BiFilter(Succ, Fail);

        [Pure]
        public static Try<T> bifilter<T>(Try<T> self, Func<T, bool> Succ, Func<Exception, bool> Fail) =>
            self.BiFilter(Succ, Fail);

        [Pure]
        public static Try<R> bind<T, R>(Try<T> tryDel, Func<T, Try<R>> binder) =>
            tryDel.Bind(binder);

        [Obsolete]
        public static Try<R> bind<T, R>(Try<T> self, Func<T, Try<R>> Succ, Func<Exception, Try<R>> Fail) =>
            self.BiBind(Succ, Fail);

        [Pure]
        public static Try<R> bibind<T, R>(Try<T> self, Func<T, Try<R>> Succ, Func<Exception, Try<R>> Fail) =>
            self.BiBind(Succ, Fail);

        [Pure]
        public static Lst<Either<Exception, T>> toList<T>(Try<T> tryDel) =>
            tryDel.ToList();

        [Pure]
        public static Either<Exception, T>[] toArray<T>(Try<T> tryDel) =>
            tryDel.ToArray();

        [Pure]
        public static IQueryable<Either<Exception, T>> toQuery<T>(Try<T> tryDel) =>
            tryDel.ToList().AsQueryable();

        [Pure]
        public static Try<T> tryfun<T>(Func<Try<T>> tryDel) => 
            Try(() => tryDel().Run());

        [Pure]
        public static Try<T> Try<T>(Func<T> tryDel) => new Try<T>(tryDel);

    }
}
