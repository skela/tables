//
//  TableUtils.h
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

typedef NS_ENUM(NSUInteger, TableRowType)
{
    TableRowTypeUnknown,
    TableRowTypeText,
    TableRowTypeBlurb,
    TableRowTypeCheckbox,
    TableRowTypeDate,
    TableRowTypeTime,
    TableRowTypeDateTime,
    TableRowTypeSingleChoiceList
};

typedef NS_ENUM(NSUInteger, KeyboardType)
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
};

typedef NS_ENUM(NSUInteger, CapitalizationType)
{
    CapitalizationTypeIgnore,
    CapitalizationTypeNone,
    CapitalizationTypeWords,
    CapitalizationTypeSentences,
    CapitalizationTypeAllCharacters
};

typedef NS_ENUM(NSUInteger, CorrectionType)
{
    CorrectionTypeIgnore,
    CorrectionTypeDefault,
    CorrectionTypeNo,
    CorrectionTypeYes
};

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
- (void)add:(NSString*)key config:(NSDictionary*)config;
@end

@interface TextHelper : NSObject
+ (NSString*)scrambledText:(NSString*)str;
+ (CGSize)stringSize:(NSString*)text font:(UIFont*)font constraint:(CGSize)constraint lineBreakMode:(NSLineBreakMode)lineBreakMode;
+ (CGSize)stringSize:(NSString*)text font:(UIFont*)font;
@end

@interface TableUtils : NSObject

+ (BOOL)hasObject:(NSDictionary*)d ofClass:(Class)classe forKey:(NSString*)key;
+ (id)getObject:(NSDictionary*)d ofClass:(Class)classe forKey:(NSString*)key fallback:(id)fallBack;
+ (NSValue*)getValue:(NSDictionary*)d forKey:(NSString*)key fallback:(NSValue*)fallBack;
+ (NSNumber*)getNumber:(NSDictionary*)d forKey:(NSString*)key fallback:(NSNumber*)fallBack;
+ (NSString*)getString:(NSDictionary*)d forKey:(NSString*)key fallback:(NSString*)fallBack;
+ (NSArray*)getArray:(NSDictionary*)d forKey:(NSString*)key fallback:(NSArray*)fallBack;
+ (SEL)getSelector:(NSDictionary*)d forKey:(NSString*)key fallback:(SEL)fallBack;

@end
