# VSMG
Generator of simple models for VS Model Creator. Creates only basic shapes and assigns uvs and textures, the rest of the refinement is done in VSMC.

### How it works
A generator consists of a main application and generator plug-ins. Dll generators located in the `generators` folder are connected at application start.

In the application you can select a generator and configure it. Additionally you can customize textures and root object. To export the generated model file click "Generate".

### Sphere generator
It is possible to choose:
- Radius
- Even
- Offset
The first texture is selected.
An additional option is a hollow sphere, you can choose an inner radius.

### Cylinder generator
Compared to the sphere, 2 more parameters are added:
- Length
- Vertical axis
It also has a hollow version (i.e. a pipe).
