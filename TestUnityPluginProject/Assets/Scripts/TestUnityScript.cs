using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

public class TestUnityScript : MonoBehaviour {
	
	#region native functions
	[DllImport("TestUnityPlugin")]
	public static extern float RandomVectorScalarProductNative(float[] fa1, float[] fa2, int N);

	[DllImport("TestUnityPlugin")]
	public static extern float RandomMatrixProductNative(float[] fa1, float[] fa2, int N);
	#endregion

	#region variables
	[Header("Number of samples")]
	public int samplesNumber;
	[Header("Mean over each sample")]
	[Range(1,100)]
	public int meanNumber;

	[Header("Dimension range for scalar product test")]
	public int minScalar, maxScalar;

	[Header("Dimension range for matrix product test")]
	public int minMatrix, maxMatrix;

	[Header("A name for the data file...?")]
	public string fileName;

	//Data info
	int[] indexes;
	float[] timescalarcsharp;
	float[] timescalarnative;
	float[] timematrixcsharp;
	float[] timematrixnative;

	//Used to compute time between events
	Stopwatch sw;
	#endregion


	//It's normal that Unity freeze a few seconds at starting point (depends on the number of samples/vector dimension
	void Start(){
		InitializeAll();
		MakeComputations();
	}

	void InitializeAll(){
		sw = new Stopwatch();
		indexes = new int[samplesNumber];
		timescalarcsharp = new float[samplesNumber];
		timescalarnative = new float[samplesNumber];
		timematrixcsharp = new float[samplesNumber];
		timematrixnative = new float[samplesNumber];
	}

	void MakeComputations(){

		string fileName = Application.dataPath+"/Data/"+this.fileName+".txt";
		if (!File.Exists(fileName))
		{

			var sr = File.CreateText(fileName);

			for(int i=0;i<samplesNumber;i++){
				indexes[i] = minScalar + (maxScalar-minScalar)*i/(samplesNumber-1);
				float[] f = MeanRandomVectorScalarProduct(meanNumber,indexes[i]);
				timescalarcsharp[i] = f[0];
				timescalarnative[i] = f[1];

				indexes[i] = minMatrix + (maxMatrix-minMatrix)*i/(samplesNumber-1);
				f = MeanRandomMatrixProduct(meanNumber,indexes[i]);
				timematrixcsharp[i] = f[0];
				timematrixnative[i] = f[1];

				sr.WriteLine((minScalar + (maxScalar-minScalar)*i/(samplesNumber-1)).ToString() + ";" + 
					timescalarcsharp[i].ToString() + ";" +
					timescalarnative[i].ToString() + ";" +
					(minMatrix + (maxMatrix-minMatrix)*i/(samplesNumber-1)).ToString() + ";" +
					timematrixcsharp[i].ToString() + ";" +
					timematrixnative[i].ToString());
			}
				
			sr.Close();

		}else{
			print("Wild error appeared!");
		}

	}


	float[] MeanRandomVectorScalarProduct(int M,int N){
		float[] f = new float[2];
		for(int i=0;i<M;i++){
			f[0]+=RandomVectorScalarProduct(N);
			f[1]+=RandomVectorScalarProductNativeWrapper(N);
		}

		f[0]/=M;
		f[1]/=M;
		return f;
	}

	float[] MeanRandomMatrixProduct(int M,int N){
		float[] f = new float[2];
		for(int i=0;i<M;i++){
			f[0]+=RandomMatrixProduct(N);
			f[1]+=RandomMatrixProductNativeWrapper(N);
		}

		f[0]/=M;
		f[1]/=M;
		return f;
	}

	float RandomVectorScalarProductNativeWrapper(int N){
		float[] fa1 = new float[N];
		float[] fa2 = new float[N];

		//Initialize arrays
		for(int i=0;i<N;i++){
			fa1[i] = Random.value;
			fa2[i] = Random.value;
		}

		return 1000f*RandomVectorScalarProductNative(fa1,fa2,N);

	}

	float RandomMatrixProductNativeWrapper(int N){
		float[] fa1 = new float[N*N];
		float[] fa2 = new float[N*N];

		//Initialize arrays
		for(int i=0;i<N;i++){
			for(int j=0;j<N;j++){
				fa1[j+i*N] = Random.value;
				fa2[j+i*N] = Random.value;
			}
		}

		return 1000f*RandomMatrixProductNative(fa1,fa2,N);

	}

	float RandomVectorScalarProduct(int N){

		float[] fa1 = new float[N];
		float[] fa2 = new float[N];

		//Initialize arrays
		for(int i=0;i<N;i++){
			fa1[i] = Random.value;
			fa2[i] = Random.value;
		}

		sw.Start();

		//Da algorithm
		float sp = 0f;
		for(int i=0;i<N;i++){
			sp+= fa1[i]*fa2[i];
		}

		sw.Stop();

		float t = (float)sw.Elapsed.Ticks/Stopwatch.Frequency * 1000f;

		sw.Reset();

		return t;
	
	}

	float RandomMatrixProduct(int N){

		float[] fa1 = new float[N*N];
		float[] fa2 = new float[N*N];

		//Initialize arrays
		for(int i=0;i<N;i++){
			for(int j=0;j<N;j++){
				fa1[j+i*N] = Random.value;
				fa2[j+i*N] = Random.value;
			}
		}

		sw.Start();

		//Da algorithm
		float[] mp = new float[N*N];
		for(int i=0;i<N;i++){
			for(int j=0;j<N;j++){
				mp[j+i*N] = 0f;
				for(int k=0;k<N;k++){
					mp[j+i*N] += fa1[k+i*N]*fa2[j+k*N];
				}

			}
		}

		sw.Stop();

		float t = (float)sw.Elapsed.Ticks/Stopwatch.Frequency * 1000f;

		sw.Reset();

		return t;
	}

}
