using UnityEngine;
using System;
using System.Threading;
using NUnit.Framework;

namespace Leap.Unity.Tests {

  public class ProduceConsumeBufferTest : MonoBehaviour {

    private ProduceConsumeBuffer<TestStruct> buffer;

    [SetUp]
    public void Setup() {
      buffer = new ProduceConsumeBuffer<TestStruct>(1024);
    }

    [TearDown]
    public void Teardown() {
      buffer = null;
    }

    [Test]
    public void Test() {
      Thread consumer = new Thread(new ThreadStart(consumerThread));
      Thread producer = new Thread(new ThreadStart(producerThread));

      consumer.Start();
      producer.Start();

      consumer.Join();
      producer.Join();
    }

    private void consumerThread() {
      try {
        for (int i = 0; i < buffer.Length; i++) {
          TestStruct s;
          s.index = i;
          s.name = i.ToString();
          buffer.TryPush(ref s);
        }
      } catch (Exception e) {
        Assert.Fail(e.Message);
      }
    }

    private void producerThread() {
      try {
        for (int i = 0; i < buffer.Length; i++) {
          TestStruct s;
          while (!buffer.TryPop(out s)) { }

          Assert.That(s.index, Is.EqualTo(i));
          Assert.That(s.name, Is.EqualTo(i.ToString()));
        }
      } catch (Exception e) {
        Assert.Fail(e.Message);
      }
    }

    private struct TestStruct {
      public int index;
      public string name;
    }
  }
}
