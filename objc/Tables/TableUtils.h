//
//  TableUtils.h
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef enum
{
    TableRowTypeUnknown,
    TableRowTypeText,
    TableRowTypeBlurb,
    TableRowTypeCheckbox,
    TableRowTypeDate,
    TableRowTypeTime,
    TableRowTypeDateTime,
    TableRowTypeSingleChoiceList,
} TableRowType;

typedef enum
{
    KeyboardTypeIgnore,
    KeyboardTypeDefault,
    KeyboardTypeASCIICapable,
    KeyboardTypeNumbersAndPunctuation,
    KeyboardTypeUrl,
    KeyboardTypeNumberPad,
    KeyboardTypePhonePad,
    KeyboardTypeNamePhonePad,
    KeyboardTypeEmailAddress,
    KeyboardTypeDecimalPad,
} KeyboardType;

typedef enum
{
    CapitalizationTypeIgnore,
    CapitalizationTypeNone,
    CapitalizationTypeWords,
    CapitalizationTypeSentences,
    CapitalizationTypeAllCharacters
} CapitalizationType;

typedef enum
{
    CorrectionTypeIgnore,
    CorrectionTypeDefault,
    CorrectionTypeNo,
    CorrectionTypeYes
} CorrectionType;

@protocol ITableAdapterRow
- (NSString *)tableRowName;
- (NSObject *)tableRowValue;
@end

@protocol ITableAdapterRowSelector;
@protocol ITableAdapter <NSObject>
@property (assign) id<ITableAdapterRowSelector>rowSelector;
@property (nonatomic,strong) NSObject *data;
- (void)reloadData;
@end

@protocol ITableAdapterRowChanged
- (void)rowChanged:(id<ITableAdapter>)adapter rowName:(NSString*)rowName oldValue:(NSObject*)oldValue  newValue:(NSObject*)newValue;
@end

@class TableAdapterRowConfig;
@protocol ITableAdapterRowConfigurator
- (TableAdapterRowConfig*)configForRow:(NSString*)rowName;
@end

@protocol ITableAdapterRowSelector
- (BOOL) tableAdapter:(id<ITableAdapter>)adapter didSelectRow:(NSString*)rowName;
@end

@protocol TableAdapterItemSelector
- (void)didSelectItem:(NSObject*)item;
@end

@protocol TableAdapterItemInformer
- (NSString*)itemText:(NSObject*)item;
- (NSString*)itemDetails:(NSObject*)item;
@end

@interface TableAdapterRowConfig : NSObject
@property(nonatomic,readwrite) BOOL editable;
@property(nonatomic,assign) SEL clicked;
@property(nonatomic,readwrite) TableRowType rowType;
@property(nonatomic,strong) NSString *displayName;
@property(nonatomic,strong) NSString *dateFormat;
@property(nonatomic,strong) NSString *timeFormat;
@property(nonatomic,strong) NSString *dateTimeFormat;
@property(nonatomic,readwrite) BOOL simpleCheckbox;
@property(nonatomic,readwrite) BOOL inlineTextEditing;
@property(nonatomic,readwrite) BOOL secureTextEditing;
@property(nonatomic,readwrite) KeyboardType keyboardType;
@property(nonatomic,readwrite) CorrectionType correctionType;
@property(nonatomic,readwrite) CapitalizationType capitalizationType;
@property(nonatomic,strong) NSArray *singleChoiceOptions;
@end

@interface TableAdapterRowConfigs : NSObject <ITableAdapterRowConfigurator>
@property(nonatomic,strong) NSMutableDictionary *configs;
@end

@interface TextHelper : NSObject
+ (NSString*)scrambledText:(NSString*)str;
@end

@interface TableHelper : NSObject
+ (CGSize)stringSize:(NSString*)text font:(UIFont*)font constraint:(CGSize)constraint lineBreakMode:(UILineBreakMode)lineBreakMode;
+ (CGSize)stringSize:(NSString*)text font:(UIFont*)font;
@end
