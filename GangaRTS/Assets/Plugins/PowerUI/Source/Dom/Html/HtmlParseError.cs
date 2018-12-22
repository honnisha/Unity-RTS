using System;


namespace Dom{
	
	/// <summary>
	/// A collection of HTML parse error codes.
	/// </summary>
	public enum HtmlParseError : byte{
		
		/// <summary>
		/// Unexpected end of file detected.
		/// </summary>
		EOF = 0,
		/// <summary>
		/// NULL character replaced by repl. character.
		/// </summary>
		Null = 0x01,
		/// <summary>
		/// Bogus comment.
		/// </summary>
		BogusComment = 0x1a,
		/// <summary>
		/// Ambiguous open tag.
		/// </summary>
		AmbiguousOpenTag = 0x1b,
		/// <summary>
		/// The tag has been closed unexpectedly.
		/// </summary>
		TagClosedWrong = 0x1c,
		/// <summary>
		/// The closing slash has been misplaced.
		/// </summary>
		ClosingSlashMisplaced = 0x1d,
		/// <summary>
		/// Undefined markup declaration found.
		/// </summary>
		UndefinedMarkupDeclaration = 0x1e,
		/// <summary>
		/// Comment ended with an exclamation mark.
		/// </summary>
		CommentEndedWithEM = 0x1f,
		/// <summary>
		/// Comment ended with a dash.
		/// </summary>
		CommentEndedWithDash = 0x20,
		/// <summary>
		/// Comment ended with an unexpected character.
		/// </summary>
		CommentEndedUnexpected = 0x21,
		/// <summary>
		/// The given tag cannot be self-closed.
		/// </summary>
		TagCannotBeSelfClosed = 0x22,
		/// <summary>
		/// End tags can never be self-closed.
		/// </summary>
		EndTagCannotBeSelfClosed = 0x23,
		/// <summary>
		/// End tags cannot carry attributes.
		/// </summary>
		EndTagCannotHaveAttributes = 0x24,
		/// <summary>
		/// No caption tag has been found within the local scope.
		/// </summary>
		CaptionNotInScope = 0x25,
		/// <summary>
		/// No select tag has been found within the local scope.
		/// </summary>
		SelectNotInScope = 0x26,
		/// <summary>
		/// No table row has been found within the local scope.
		/// </summary>
		TableRowNotInScope = 0x27,
		/// <summary>
		/// No table has been found within the local scope.
		/// </summary>
		TableNotInScope = 0x28,
		/// <summary>
		/// No paragraph has been found within the local scope.
		/// </summary>
		ParagraphNotInScope = 0x29,
		/// <summary>
		/// No body has been found within the local scope.
		/// </summary>
		BodyNotInScope = 0x2a,
		/// <summary>
		/// No block element has been found within the local scope.
		/// </summary>
		BlockNotInScope = 0x2b,
		/// <summary>
		/// No table cell has been found within the local scope.
		/// </summary>
		TableCellNotInScope = 0x2c,
		/// <summary>
		/// No table section has been found within the local scope.
		/// </summary>
		TableSectionNotInScope = 0x2d,
		/// <summary>
		/// No object element has been found within the local scope.
		/// </summary>
		ObjectNotInScope = 0x2e,
		/// <summary>
		/// No heading element has been found within the local scope.
		/// </summary>
		HeadingNotInScope = 0x2f,
		/// <summary>
		/// No list item has been found within the local scope.
		/// </summary>
		ListItemNotInScope = 0x30,
		/// <summary>
		/// No form has been found within the local scope.
		/// </summary>
		FormNotInScope = 0x31,
		/// <summary>
		/// No button has been found within the local scope.
		/// </summary>
		ButtonInScope = 0x32,
		/// <summary>
		/// No nobr element has been found within the local scope.
		/// </summary>
		NobrInScope = 0x33,
		/// <summary>
		/// No element has been found within the local scope.
		/// </summary>
		ElementNotInScope = 0x34,
		/// <summary>
		/// Character reference found no numbers.
		/// </summary>
		CharacterReferenceWrongNumber = 0x35,
		/// <summary>
		/// Character reference found no semicolon.
		/// </summary>
		CharacterReferenceSemicolonMissing = 0x36,
		/// <summary>
		/// Character reference within an invalid range.
		/// </summary>
		CharacterReferenceInvalidRange = 0x37,
		/// <summary>
		/// Character reference is an invalid number.
		/// </summary>
		CharacterReferenceInvalidNumber = 0x38,
		/// <summary>
		/// Character reference is an invalid code.
		/// </summary>
		CharacterReferenceInvalidCode = 0x39,
		/// <summary>
		/// Character reference is not terminated by a semicolon.
		/// </summary>
		CharacterReferenceNotTerminated = 0x3a,
		/// <summary>
		/// Character reference in attribute contains an invalid character (=).
		/// </summary>
		CharacterReferenceAttributeEqualsFound = 0x3b,
		/// <summary>
		/// The specified item has not been found.
		/// </summary>
		ItemNotFound = 0x3c,
		/// <summary>
		/// The encoding operation (either encoded or decoding) failed.
		/// </summary>
		EncodingError = 0x3d,
		/// <summary>
		/// Doctype unexpected character after the name detected.
		/// </summary>
		DoctypeUnexpectedAfterName = 0x40,
		/// <summary>
		/// Invalid character in the public identifier detected.
		/// </summary>
		DoctypePublicInvalid = 0x41,
		/// <summary>
		/// Invalid character in the doctype detected.
		/// </summary>
		DoctypeInvalidCharacter = 0x42,
		/// <summary>
		/// Invalid character in the system identifier detected.
		/// </summary>
		DoctypeSystemInvalid = 0x43,
		/// <summary>
		/// The doctype tag is misplaced and ignored.
		/// </summary>
		DoctypeTagInappropriate = 0x44,
		/// <summary>
		/// The given doctype tag is invalid.
		/// </summary>
		DoctypeInvalid = 0x45,
		/// <summary>
		/// Doctype encountered unexpected character.
		/// </summary>
		DoctypeUnexpected = 0x46,
		/// <summary>
		/// The doctype tag is missing.
		/// </summary>
		DoctypeMissing = 0x47,
		/// <summary>
		/// The given public identifier for the notation declaration is invalid.
		/// </summary>
		NotationPublicInvalid = 0x48,
		/// <summary>
		/// The given system identifier for the notation declaration is invalid.
		/// </summary>
		NotationSystemInvalid = 0x49,
		/// <summary>
		/// The type declaration is missing a valid definition.
		/// </summary>
		TypeDeclarationUndefined = 0x4a,
		/// <summary>
		/// A required quantifier is missing in the provided expression.
		/// </summary>
		QuantifierMissing = 0x4b,
		/// <summary>
		/// The double quotation marks have been misplaced.
		/// </summary>
		DoubleQuotationMarkUnexpected = 0x50,
		/// <summary>
		/// The single quotation marks have been misplaced.
		/// </summary>
		SingleQuotationMarkUnexpected = 0x51,
		/// <summary>
		/// The attribute's name contains an invalid character.
		/// </summary>
		AttributeNameInvalid = 0x60,
		/// <summary>
		/// The attribute's value contains an invalid character.
		/// </summary>
		AttributeValueInvalid = 0x61,
		/// <summary>
		/// The beginning of a new attribute has been expected.
		/// </summary>
		AttributeNameExpected = 0x62,
		/// <summary>
		/// The attribute has already been added.
		/// </summary>
		AttributeDuplicateOmitted = 0x63,
		/// <summary>
		/// The given tag must be placed in head tag.
		/// </summary>
		TagMustBeInHead = 0x70,
		/// <summary>
		/// The given tag is not appropriate for the current position.
		/// </summary>
		TagInappropriate = 0x71,
		/// <summary>
		/// The given tag cannot end at the current position.
		/// </summary>
		TagCannotEndHere = 0x72,
		/// <summary>
		/// The given tag cannot start at the current position.
		/// </summary>
		TagCannotStartHere = 0x73,
		/// <summary>
		/// The given form cannot be placed at the current position.
		/// </summary>
		FormInappropriate = 0x74,
		/// <summary>
		/// The given input cannot be placed at the current position.
		/// </summary>
		InputUnexpected = 0x75,
		/// <summary>
		/// The closing tag and the currently open tag do not match.
		/// </summary>
		TagClosingMismatch = 0x76,
		/// <summary>
		/// The given end tag does not match the current node.
		/// </summary>
		TagDoesNotMatchCurrentNode = 0x77,
		/// <summary>
		/// This position does not support a linebreak (LF, FF).
		/// </summary>
		LineBreakUnexpected = 0x78,
		/// <summary>
		/// The head tag can only be placed once inside the html tag.
		/// </summary>
		HeadTagMisplaced = 0x80,
		/// <summary>
		/// The html tag can only be placed once as the root element.
		/// </summary>
		HtmlTagMisplaced = 0x81,
		/// <summary>
		/// The body tag can only be placed once inside the html tag.
		/// </summary>
		BodyTagMisplaced = 0x82,
		/// <summary>
		/// The image tag has been named image instead of img.
		/// </summary>
		ImageTagNamedWrong = 0x83,
		/// <summary>
		/// Tables cannot be nested.
		/// </summary>
		TableNesting = 0x84,
		/// <summary>
		/// An illegal element has been detected in a table.
		/// </summary>
		IllegalElementInTableDetected = 0x85,
		/// <summary>
		/// Select elements cannot be nested.
		/// </summary>
		SelectNesting = 0x86,
		/// <summary>
		/// An illegal element has been detected in a select.
		/// </summary>
		IllegalElementInSelectDetected = 0x87,
		/// <summary>
		/// The frameset element has been misplaced.
		/// </summary>
		FramesetMisplaced = 0x88,
		/// <summary>
		/// Headings cannot be nested.
		/// </summary>
		HeadingNested = 0x89,
		/// <summary>
		/// Anchor elements cannot be nested.
		/// </summary>
		AnchorNested = 0x8a,
		/// <summary>
		/// The given token cannot be inserted here.
		/// </summary>
		TokenNotPossible = 0x90,
		/// <summary>
		/// The current node is not the root element.
		/// </summary>
		CurrentNodeIsNotRoot = 0x91,
		/// <summary>
		/// The current node is the root element.
		/// </summary>
		CurrentNodeIsRoot = 0x92,
		/// <summary>
		/// This tag is invalid in fragment mode.
		/// </summary>
		TagInvalidInFragmentMode = 0x93,
		/// <summary>
		/// There is already an open form.
		/// </summary>
		FormAlreadyOpen = 0x94,
		/// <summary>
		/// The form has been closed wrong.
		/// </summary>
		FormClosedWrong = 0x95,
		/// <summary>
		/// The body has been closed wrong.
		/// </summary>
		BodyClosedWrong = 0x96,
		/// <summary>
		/// An expected formatting element has not been found.
		/// </summary>
		FormattingElementNotFound = 0x97
		
	}
	
}
