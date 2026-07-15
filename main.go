package main

// IMPLEMENTACJA BARIERY

import (
	"fmt"
	"math/rand"
	"sync"
	"time"
)

type Barrier struct {
	lock          sync.Mutex
	barrier_count int
	thread_count  int
}

func barrier_init(thread_count int) *Barrier {
	return &Barrier{
		lock:          sync.Mutex{},
		barrier_count: 0,
		thread_count:  thread_count,
	}
}

func (b *Barrier) barrier_wait() {
	b.lock.Lock()
	b.barrier_count++
	b.lock.Unlock()

	// active waiting
	for {
		if b.barrier_count >= b.thread_count {
			b.lock.Lock()
			b.barrier_count = 0
			b.lock.Unlock()
			break
		}
	}
}

func doSomething() {
	defer wg.Done()
	fmt.Println("Doing something...")
	time.Sleep(time.Millisecond * (time.Duration(rand.Int31n(2000) + 100)))
	fmt.Println("Done something, waiting for barrier")
	barrier.barrier_wait()
	fmt.Println("Barrier raised, continuing")

	fmt.Println("Doing something...")
	time.Sleep(time.Millisecond * (time.Duration(rand.Int31n(2000) + 100)))
	fmt.Println("Done something, waiting for barrier")
	barrier.barrier_wait()
	fmt.Println("Barrier raised, continuing")
}

var wg sync.WaitGroup
var threadCount int = 4
var barrier *Barrier = barrier_init(threadCount)

func main() {
	defer wg.Wait()
	wg.Add(threadCount)

	for i := 0; i < threadCount; i++ {
		go doSomething()
	}

}
