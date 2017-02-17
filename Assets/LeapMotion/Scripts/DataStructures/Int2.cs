using System;
using UnityEngine;

namespace Leap.Unity {

  [Serializable]
  public struct Int2 : IEquatable<Int2> {
    public static readonly Int2 zero = new Int2(0, 0);
    public static readonly Int2 one = new Int2(1, 1);
    public static readonly Int2 up = new Int2(0, 1);
    public static readonly Int2 down = new Int2(0, -1);
    public static readonly Int2 left = new Int2(-1, 0);
    public static readonly Int2 right = new Int2(1, 0);

    public int x, y;

    public Int2(int x, int y) {
      this.x = x;
      this.y = y;
    }

    public float magnitude {
      get {
        return Mathf.Sqrt(x * x + y * y);
      }
    }

    public int sqrMagnitude {
      get {
        return x * x + y * y;
      }
    }

    public int this[int index] {
      get {
        switch (index) {
          case 0:
            return x;
          case 1:
            return y;
          default:
            throw new IndexOutOfRangeException();
        }
      }
    }

    public void Set(int x, int y) {
      this.x = x;
      this.y = y;
    }

    public void Scale(Int2 other) {
      x *= other.x;
      y *= other.y;
    }

    public static float Distance(Int2 a, Int2 b) {
      return (a - b).magnitude;
    }

    public static Int2 Min(Int2 a, Int2 b) {
      return new Int2(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
    }

    public static Int2 Max(Int2 a, Int2 b) {
      return new Int2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
    }

    public static Int2 Scale(Int2 a, Int2 b) {
      return new Int2(a.x * b.x, a.y * b.y);
    }

    public static Int2 operator +(Int2 a, Int2 b) {
      return new Int2(a.x + b.x, a.y + b.y);
    }

    public static Int2 operator -(Int2 a, Int2 b) {
      return new Int2(a.x - b.x, a.y - b.y);
    }

    public static Int2 operator -(Int2 a) {
      return new Int2(-a.x, -a.y);
    }

    public static Int2 operator *(Int2 a, int scalar) {
      return new Int2(a.x * scalar, a.y * scalar);
    }

    public static Int2 operator *(int scalar, Int2 a) {
      return new Int2(a.x * scalar, a.y * scalar);
    }

    public static Int2 operator /(Int2 a, int denominator) {
      return new Int2(a.x / denominator, a.y / denominator);
    }

    public static Int2 operator /(int numerator, Int2 a) {
      return new Int2(numerator / a.x, numerator / a.y);
    }

    public static bool operator ==(Int2 a, Int2 b) {
      return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Int2 a, Int2 b) {
      return a.x != b.x || a.y != b.y;
    }

    public static implicit operator Vector2(Int2 a) {
      return new Vector2(a.x, a.y);
    }

    public override bool Equals(object obj) {
      if (!(obj is Int2)) {
        return false;
      }
      return this == (Int2)obj;
    }

    public override int GetHashCode() {
      return x * 23 + y;
    }

    public override string ToString() {
      return "(" + x + ", " + y + ")";
    }

    public bool Equals(Int2 other) {
      return this == other;
    }
  }
}
