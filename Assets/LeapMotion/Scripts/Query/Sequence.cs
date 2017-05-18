using System;

namespace Leap.Unity.Query {

  public struct SequenceGenOp<ResultType> : IQueryOp<ResultType> {

    private ResultType _seed;
    private ResultType _previous;
    private Func<ResultType, ResultType> _getNext;

    public SequenceGenOp(ResultType seed, Func<ResultType, ResultType> getNext) {
      _seed = seed;
      _previous = seed;
      _getNext = getNext;
    }

    public bool TryGetNext(out ResultType t) {
      t = _previous = _getNext(_previous);
      return true;
    }

    public void Reset() {
      _previous = _seed;
    }
  }

  public struct SingleOp<ResultType> : IQueryOp<ResultType> {
    private ResultType _value;
    private bool _hasRun;

    public SingleOp(ResultType value) {
      _value = value;
      _hasRun = false;
    }

    public bool TryGetNext(out ResultType t) {
      if (!_hasRun) {
        t = _value;
        _hasRun = true;
        return true;
      } else { 
        t = default(ResultType);
        return false;
      }
    }

    public void Reset() {
      _hasRun = false;
    }
  }

  public struct EmptySequenceOp<ResultType> : IQueryOp<ResultType> {

    public bool TryGetNext(out ResultType t) {
      t = default(ResultType);
      return false;
    }

    public void Reset() { }
  }

  public static class Sequence {

    public static QueryWrapper<T, EmptySequenceOp<T>> Empty<T>() {
      return new QueryWrapper<T, EmptySequenceOp<T>>(new EmptySequenceOp<T>());
    }

    public static QueryWrapper<T, >

    public static QueryWrapper<T, SequenceGenOp<T>> Repeat<T>(T t) {
      return new QueryWrapper<T, SequenceGenOp<T>>(new SequenceGenOp<T>(t, i => i));
    }

    public static QueryWrapper<int, SequenceGenOp<int>> NumbersFrom(int start, int to = int.MaxValue, int step = 1, bool inclusive = false) {
      return new QueryWrapper<int, SequenceGenOp<int>>(new SequenceGenOp<int>(start, i => i + step));
    }

    /*
    public static QueryWrapper<float, SequenceGenOp<float>> Ascending(float start, float step = 1) {
      return new QueryWrapper<float, SequenceGenOp<float>>(new SequenceGenOp<float>(start, i => i + step));
    }

    public static QueryWrapper<int, SequenceGenOp<int>> Descending(int start, int step = 1) {
      return Ascending(start, -step);
    }

    public static QueryWrapper<float, SequenceGenOp<float>> Descending(float start, float step = 1) {
      return Ascending(start, -step);
    }

    public static QueryWrapper<int, TakeCountOp<int, SequenceGenOp<int>>> Range(int count) {
      return Ascending(0).Take(count);
    }

    public static QueryWrapper<int, TakeCountOp<int, SequenceGenOp<int>>> Range(int start, int count) {
      return Ascending(start).Take(count);
    }

    public static QueryWrapper<int, TakeCountOp<int, SequenceGenOp<int>>> Range(int start, int count, int step) {
      return Ascending(start, step).Take(count);
    }
    */
  }
}
