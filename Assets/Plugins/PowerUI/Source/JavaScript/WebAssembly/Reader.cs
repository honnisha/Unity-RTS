using System;
using System.IO;
using System.Reflection;


namespace WebAssembly{
	
	/// <summary>
	/// This class is a wrapper for any block of bytes. It allows values to be read from the stream
	/// one at a time.
	/// </summary>

	public class Reader : System.IO.BinaryReader{
		
		/// <summary>The current module being read from.</summary>
		public Module Module;
		/// <summary>The current instruction being read.</summary>
		internal Instruction Instruction;
		/// <summary>The current function body being read.</summary>
		internal FunctionBody FunctionBody;
		/// <summary>Labelled block stack.</summary>
		public CompilerStack<LabelledBlock> BlockStack = new CompilerStack<LabelledBlock>();
		
		
		/// <summary>Gets a labelled block by label index.</summary>
		public LabelledBlock GetBlock(int index){
			return BlockStack.Peek(index);
		}
		
		/// <summary>Pushes a label onto the label stack.</summary>
		public LabelledBlock PushBlock(Type blockReturnType){
			LabelledBlock block = new LabelledBlock(blockReturnType,Instruction);
			BlockStack.Push(block);
			return block;
		}
		
		/// <summary>Pops a labelled block from the stack.</summary>
		public LabelledBlock PopBlock(){
			
			// Pop it:
			LabelledBlock block = BlockStack.Pop();
			Instruction blockInstruction = block.Instruction;
			
			// Transfer all instructions on the FunctionBody.Root stack
			// into blockInstruction up to but not 
			// including a match on blockInstruction.
			
			CompilerStack<Instruction> stack = FunctionBody.Root;
			
			for(int i=0;i<stack.Count;i++){
				
				if(stack.Peek()==blockInstruction){
					break;
				}
				
				// Push it into the block:
				blockInstruction.PrependInput(stack.Pop());
				
			}
			
			return block;
		}
		
		/// <summary>Creates a new reader for the given stream.</summary>
		/// <param name="stream">A stream to read from.</param>
		public Reader(Stream stream):base(stream){}
		
		/// <summary>Creates a new reader for the given block of bytes.</summary>
		/// <param name="src">The source block of bytes to read from.</param>
		public Reader(byte[] src):base(new MemoryStream(src)){}
		
		
		/// <summary>Reads a uint8.</summary>
		public uint UInt8(){
			return (uint)ReadByte();
		}
		
		/// <summary>Reads a uint16 (little endian).</summary>
		public uint UInt16(){
			return (uint)ReadByte() | (uint)ReadByte()<<8;
		}
		
		/// <summary>Reads a uint32 (little endian).</summary>
		public uint UInt32(){
			return	(uint)ReadByte() | (uint)ReadByte()<<8 | 
					(uint)ReadByte()<<16 | (uint)ReadByte()<<24;
		}
		
		/// <summary>Reads a uint64 (little endian).</summary>
		public ulong UInt64(){
			return	(ulong)ReadByte() | (ulong)ReadByte()<<8 | 
					(ulong)ReadByte()<<16 | (ulong)ReadByte()<<24 |
					(ulong)ReadByte()<<32 | (ulong)ReadByte()<<40 | 
					(ulong)ReadByte()<<48 | (ulong)ReadByte()<<56;
		}
		
		/// <summary>Reads a method from the modules function index space.</summary>
		public MethodInfo Method(out int paramCount,out Type returnType){
			return Module.GetFunction((int)VarUInt32(),out paramCount,out returnType);
		}
		
		/// <summary>Reads a function type reference.</summary>
		public FuncType FuncType(){
			return Module.FunctionSection.Types[(int)VarUInt32()];
		}
		
		/// <summary>Reads a memory_immediate and emits the offset straight away.</summary>
		public void MemoryAddress(ILGenerator gen,int logNaturalSize){
			
			// Read the memory_immediate:
			uint flags = VarUInt32();
			uint offset = VarUInt32();
			
			if(offset!=0){
				// Add the offset to the effective address:
				gen.LoadInt32(offset);
				gen.Add();
			}
			
			// Alignment test next - check if bottom 2 bits of flags == logNaturalSize:
			int logAlignment=(int)(flags & 4);
			
			if(logAlignment != logNaturalSize){
				
				// It's an unaligned access. Emit the actual alignment:
				gen.Unaligned( (byte)( 1<<logAlignment ) );
				
				// logAlignment of 0 => align value of 1
				// logAlignment of 1 => align value of 2
				// logAlignment of 2 => align value of 4
				// logAlignment of 3 => align value of 8 (should be never used)
				
			}
			
		}
		
		/// <summary>Reads a memory immediate.</summary>
		public MemoryImmediate Memory(){
			// memory_immediate
			uint flags = VarUInt32();
			uint offset = VarUInt32();
			return new MemoryImmediate(flags, offset);
		}
		
		/// <summary>Reads a labelled block reference.</summary>
		public LabelledBlock ReadLabelledBlock(){
			return BlockStack.Peek((int)VarUInt32());
		}
		
		/// <summary>Current position.</summary>
		public long Position{
			get{
				return BaseStream.Position;
			}
		}
		
		/// <summary>Seeks over the given number of bytes.</summary>
		public void Skip(int amount){
			BaseStream.Seek(amount,SeekOrigin.Current);
		}
		
		/// <summary>Seeks to the given start-relative point.</summary>
		public void Seek(long to){
			BaseStream.Seek(to,SeekOrigin.Begin);
		}
		
		/// <summary>Variable length integer. LEB128.</summary>
		public ulong VarUInt64(){
			int offset = 0;
			ulong result = 0;
			
			// Limit to ceil(64/7) bytes:
			while(offset<=63){
				
				byte c = ReadByte();
				
				// Apply 7 bits:
				result |= (ulong)(c & 0x7f) << offset;
				
				// Done?
				if ((c & 0x80) == 0){
					break;
				}
				
				offset += 7;
			}
			
			return result;
		}
		
		/// <summary>Variable length integer. LEB128.</summary>
		public uint VarUInt32(){
			int offset = 0;
			uint result = 0;
			
			// Limit to ceil(32/7) bytes:
			while(offset<=28){
				
				byte c = ReadByte();
				
				// Apply 7 bits:
				result |= (uint)(c & 0x7f) << offset;
				
				// Done?
				if ((c & 0x80) == 0){
					break;
				}
				
				offset += 7;
			}
			
			return result;
		}
		
		/// <summary>Reads a varuint32, then reads a block of bytes that long as a string.</summary>
		public string String(){
			// Assume UTF8:
			return System.Text.Encoding.UTF8.GetString(ReadBytes((int)VarUInt32()));
		}
		
		/// <summary>Reads a string of the given length.</summary>
		public string String(uint length){
			// Assume UTF8:
			return System.Text.Encoding.UTF8.GetString(ReadBytes((int)length));
		}
		
		/// <summary>Reads a resizable_limits tuple.</summary>
		public ResizableLimits Limits(){
			
			bool hasMax=VarUInt1();
			
			return new ResizableLimits(
				hasMax,
				VarUInt32(),
				hasMax ? VarUInt32() : 0
			);
			
		}
		
		/// <summary>Called when this reader fails.</summary>
		public void Failed(string str){
			throw new Exception(str);
		}
		
		/// <summary>Reads a set of func_types.</summary>
		public FuncType[] FuncTypes(int count,int globalOffset){
			
			// Create
			FuncType[] set=new FuncType[count];
			
			// Load each one:
			for(int i=0;i<set.Length;i++){
				
				// Load it:
				set[i]=new FuncType(this,i+globalOffset);
				
			}
			
			return set;
			
		}
		
		/// <summary>Reads a global variable reference.</summary>
		public FieldInfo Global(out Type returnType){
			return Module.GetGlobal((int)VarUInt32(),out returnType);
		}
		
		/// <summary>Reads a set of value types.</summary>
		public LanguageType[] ValueTypes(int count){
			
			// Create the set:
			LanguageType[] results=new LanguageType[count];
			
			// Read each one:
			for(int i=0;i<count;i++){
				results[i]=(LanguageType)ReadByte();
			}
			
			return results;
			
		}
		
		/// <summary>An initialiser expression.</summary>
		public object InitExpression(){
			
			// Opcode:
			OpCode opcode=OpCodes.Get(ReadByte());
			
			if(opcode.AllowedInInit){
				
				// Read its immediates:
				object imms=opcode.ReadImmediates(this);
				
				if(ReadByte() != 0x0b){
					throw new Exception("Init expr did not end with the 'end' opcode.");
				}
				
				return imms;
				
			}
			
			throw new Exception("unexpected opcode in initializer expression ("+opcode.Code+").");
		}
		
		/// <summary>Reads a value_type converted to a suitable system type.</summary>
		public Type ValueTypeConverted(){
			return LanguageTypes.ToType( (LanguageType)ReadByte() );
		}
		
		/// <summary>Reads a value_type.</summary>
		public LanguageType ValueType(){
			return (LanguageType)ReadByte();
		}
		
		/// <summary>Variable length integer. LEB128.</summary>
		public uint VarUInt7(){
			return (uint)(ReadByte() & 0x7f);
		}
		
		/// <summary>Variable length integer. LEB128.</summary>
		public bool VarUInt1(){
			return ReadByte()==1;
		}
		
		/// <summary>Signed LEB128 integer (max size of 64 bits).</summary>
		public long VarInt64(){
			long result = 0;
			int shift = 0;
			int size = 64;
			byte c = ReadByte();
			
			while((c & 0x80) != 0){
				result |= (long)(c & 0x7f) << shift;
				shift += 7;
				c = ReadByte();
			}
			
			/* sign bit of byte is second high order bit (0x40) */
			if ((shift < size) && ((c & 0x40) != 0)){
				/* sign extend */
				result = -result;
			}
			
			return result;
		}
		
		/// <summary>Signed LEB128 integer (max size of 32 bits).</summary>
		public int VarInt32(){
			int result = 0;
			int shift = 0;
			int size = 32;
			byte c = ReadByte();
			
			while((c & 0x80) != 0){
				result |= (c & 0x7f) << shift;
				shift += 7;
				c = ReadByte();
			}
			
			/* sign bit of byte is second high order bit (0x40) */
			if ((shift < size) && ((c & 0x40) != 0)){
				/* sign extend */
				result = -result;
			}
			
			return result;
		}
		
		/// <summary>Signed LEB128 integer (max size 7 bits).</summary>
		public int VarInt7(){
			byte c = ReadByte();
			int result=(c & 0x7f);
			
			/* sign bit of byte is second high order bit (0x40) */
			if ((c & 0x40) != 0){
				/* sign extend */
				result = -result;
			}
			
			return result;
		}
		
		/// <summary>Checks if there are any more bytes in this stream.</summary>
		/// <returns>True if there is more bytes; false otherwise.</returns>
		public bool IsMore{
			get{
				return BaseStream.Position<BaseStream.Length;
			}
		}
		
	}
	
}