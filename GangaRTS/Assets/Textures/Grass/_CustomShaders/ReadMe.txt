Requires Unity 5.3.4f1 or higher!

Soft Grass Shader is a transparent grass shader,
which gives your grass soft and more realistic look
without any configuration needed. Just import the
package and refresh the terrain.
If you use any other 3rd party grass/foliage shader, you may
need to delete them in order to get Soft Grass Shader work.

Make sure that the "Billboard" option is unchecked, otherwise
the grass will use standard billboard shader.

Because the shader does not support terrain's wind feature,
you need to set all the parameters in Terrain's wind settings to 0 to
prevent any errors or glitches in shading.

Adjusting grass color works normally, except that now
you can use the Alpha slider to modify grass' transparency.

Compatibility on mobile devices has not been tested!

Limitations:
-Soft Grass works only with non-billboarding grass. 
-It does not receive nor cast shadows due to transparency. 
-Doesn't support terrain's wind feature.