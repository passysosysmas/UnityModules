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

    public static QueryWrapper<T, SequenceGenOp<T>> Value<T>(T t) {
      return new QueryWrapper<T, SequenceGenOp<T>>(new SequenceGenOp<T>(t, i => i));
    }

    public static QueryWrapper<int, SequenceGenOp<int>> Ascending(int start, int step = 1) {
      return new QueryWrapper<int, SequenceGenOp<int>>(new SequenceGenOp<int>(start, i => i + step));
    }

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
  }
}
