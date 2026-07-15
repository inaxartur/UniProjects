#include <stdio.h>
#include <stdlib.h>
#include <pthread.h>
#include <unistd.h>
#include <semaphore.h>

#define MAX_P 4
#define P_AMMOUNT 8

sem_t t_enter;
sem_t t_exit;
sem_t ride_start;
sem_t ride_end;

int counter = 0;
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

void *passenger(void *arg) {
    int id = *(int*)arg;

    sem_wait(&t_enter);

    pthread_mutex_lock(&mutex);
    ++counter;
    printf("Passenger %d entered\n", id);

    if (counter == MAX_P) {
        sem_post(&ride_start);
    }

    pthread_mutex_unlock(&mutex);

    sem_wait(&ride_end);

    printf("Passenger %d exits\n", id);


    pthread_mutex_lock(&mutex);
    --counter;

    if(counter == 0) {
        sem_post(&t_exit);
    }

    pthread_mutex_unlock(&mutex);

    free(arg);
}

void *trolley(void *arg) {
    while (1) {
        for (int i = 0; i < MAX_P; ++i) {
            sem_post(&t_enter);
        }

        sem_wait(&ride_start);

        printf("\nTrolley starts\n");
        sleep(1);
        printf("Trolley stops\n");

        for (int i = 0; i < MAX_P; ++i) {
            sem_post(&ride_end);
        }

        sem_wait(&t_exit);
    }
}

int main() {
    pthread_t passengers[P_AMMOUNT];
    pthread_t car;

    sem_init(&t_enter, 0, 0);
    sem_init(&t_exit, 0, 0);
    sem_init(&ride_start, 0, 0);
    sem_init(&ride_end, 0, 0);

    pthread_create(&car, NULL, trolley, NULL);

    for (int i = 0; i < P_AMMOUNT; ++i) {
        int *id = malloc(sizeof(int));
        *id = i + 1;
        pthread_create(&passengers[i], NULL, passenger, id);
        sleep(1);
    }

    return 0;
}