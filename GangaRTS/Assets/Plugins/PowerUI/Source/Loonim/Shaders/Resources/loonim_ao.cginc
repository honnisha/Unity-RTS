
half frag_ao (v2f_ao i, float3 samples[INPUT_SAMPLE_COUNT])
{
	// read random normal from noise texture
    half3 randN = tex2D (_Src2, i.uv).xyz * 2.0 - 1.0;    
    
    // read scene depth/normal
    float3 viewNorm = tex2D (_Src1, i.uv).rgb;
    float depth = tex2D (_Src0, i.uv).r;
    float scale = _Data.w / depth;
    
    // accumulated occlusion factor
    float occ = 0.0;
    for (int s = 0; s < INPUT_SAMPLE_COUNT; ++s)
    {
    	// Reflect sample direction around a random vector
        half3 randomDir = reflect(samples[s], randN);
        
        // Make it point to the upper hemisphere
        randomDir *= ( (dot(viewNorm,randomDir)>=0) * 2 ) - 1;
		
        // Add a bit of normal to reduce self shadowing
        randomDir += viewNorm * 0.3;
        
        float2 offset = randomDir.xy * scale;
        float sD = depth - (randomDir.z * _Data.x);

		// Sample depth at offset location
		// float3 sampleN = tex2D (_Src1, i.uv + offset).rgb;
		float sampleD = tex2D (_Src0, i.uv + offset).r;
		
        float zd = saturate(sD-sampleD);
		
		// If ZD is > minZ then this sample contributes to occlusion.
		occ+=(zd > _Data.y) * ( pow(1-zd,_Data.z) ); //+ 1.0-saturate(pow(1.0 - zd, 11.0) + zd) + 1.0/(1.0+zd*zd*10) );
        
    }
	
	occ = 1-(occ/INPUT_SAMPLE_COUNT);
    return half4(occ,occ,occ,1);
}