package main

// IMPLEMENTACJA SEMFAFORU

import (
	"fmt"
	"math/rand"
	"sync"
	"time"
)

type sem struct {
	counter uint32
	mtx     sync.Mutex
	cnd     *sync.Cond
}

var funcID = 0
var globMtx sync.Mutex
var wg sync.WaitGroup

func myfunc(semaphore *sem) {
	defer wg.Done()

	globMtx.Lock()
	var localID = funcID
	funcID++
	globMtx.Unlock()

	sem_wait(semaphore)

	fmt.Printf("funkcja %v zaczyna obliczenia\n", localID)
	for i := 0; i < 5; i++ {
		time.Sleep(time.Millisecond * (time.Duration(rand.Int31n(400) + 100)))
	}
	fmt.Printf("Funkcja %v skonczyla obliczenia\n", localID)

	sem_post(semaphore)

}

func main() {
	wg.Add(5)
	defer wg.Wait()

	var sem1 sem
	sem_init(&sem1, 2)

	go myfunc(&sem1)
	go myfunc(&sem1)
	go myfunc(&sem1)
	go myfunc(&sem1)
	go myfunc(&sem1)

}

func sem_init(s *sem, x uint32) {
	s.counter = x
	s.cnd = sync.NewCond(&s.mtx)
}

func sem_wait(semaphore *sem) {
	semaphore.mtx.Lock()
	for semaphore.counter == 0 {
		semaphore.cnd.Wait()
	}
	semaphore.counter--
	semaphore.mtx.Unlock()
}

func sem_post(semaphore *sem) {
	semaphore.mtx.Lock()
	semaphore.counter++
	semaphore.cnd.Signal()
	semaphore.mtx.Unlock()
}
