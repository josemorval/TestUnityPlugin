#include <time.h>
#include <stdlib.h>


extern "C" {

    //Native scalar product of two vectors
    float RandomVectorScalarProductNative(float *fa1, float *fa2, int N){
        
        clock_t t = clock();
        
        float sp = 0.0;
        for(int i=0;i<N;i++){
            sp+=fa1[i]*fa2[i];
        }
        
        t = clock()-t;
        return (float)t/CLOCKS_PER_SEC;
    }
    
    //Native non-efficient product of two matrices
    float RandomMatrixProductNative(float *fa1, float *fa2, int N){
        
        clock_t t = clock();
        
        float *mp = (float*)malloc(N*N*sizeof(float));
        
        for(int i=0;i<N;i++){
            for(int j=0;j<N;j++){
                mp[j+i*N] = 0.0;
                for(int k=0;k<N;k++){
                    mp[j+i*N] += fa1[k+i*N]*fa2[j+k*N];
                }
            }
        }
        
        t = clock()-t;
        return (float)t/CLOCKS_PER_SEC;
        
    }
    
    
    
}