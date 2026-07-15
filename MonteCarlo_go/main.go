package main

import (
	"fmt"
	"math/rand"
	"sync"
	"time"
)

const THREADS = 8
const AMMOUNT_OF_POINTS = 100000000 / THREADS

var wg sync.WaitGroup
var mtx sync.Mutex

var square_points int32

func main() {
	wg.Add(THREADS)

	//fmt.Println(AMMOUNT_OF_POINTS)
	//fmt.Println(runtime.NumCPU())

	square_points = THREADS * AMMOUNT_OF_POINTS
	circle_points := [THREADS]int32{}

	for i := 0; i < THREADS; i++ {
		go calcPi(&circle_points[i])
	}

	wg.Wait()

	var pi float64

	for i := 0; i < THREADS; i++ {
		pi += (float64(circle_points[i]*4) / float64(square_points))
	}

	fmt.Printf("PI: %v\n", pi)
}

func calcPi(circle_points *int32) {
	defer wg.Done()
	start := time.Now()

	var rand_y float32
	var rand_x float32
	var dis_to_origin float32
	r := rand.New(rand.NewSource(time.Now().UnixNano()))

	for i := 0; i < AMMOUNT_OF_POINTS; i++ {
		rand_y = r.Float32()
		rand_x = r.Float32()

		dis_to_origin = rand_x*rand_x + rand_y*rand_y

		if dis_to_origin <= 1 {
			(*circle_points)++
		}
	}

	fmt.Println(time.Since(start))
}
