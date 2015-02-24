//
//  TableUtils.m
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import "TableUtils.h"

@implementation TableUtils

+ (BOOL)hasObject:(NSDictionary*)d ofClass:(Class)classe forKey:(NSString*)key
{
    NSObject *obj=[d objectForKey:key];
    if (obj!=nil && [obj isKindOfClass:classe])
        return YES;
    return NO;
}

+ (id)getObject:(NSDictionary*)d ofClass:(Class)classe forKey:(NSString*)key fallback:(id)fallBack
{
    NSObject *obj=[d objectForKey:key];
    if (obj!=nil && [obj isKindOfClass:classe])
        return obj;
    return fallBack;
}

+ (NSValue*)getValue:(NSDictionary*)d forKey:(NSString*)key fallback:(NSValue*)fallBack
{
    return [TableUtils getObject:d ofClass:[NSValue class] forKey:key fallback:fallBack];
}

+ (NSNumber*)getNumber:(NSDictionary*)d forKey:(NSString*)key fallback:(NSNumber*)fallBack
{
    return [TableUtils getObject:d ofClass:[NSNumber class] forKey:key fallback:fallBack];
}

+ (NSString*)getString:(NSDictionary*)d forKey:(NSString*)key fallback:(NSString*)fallBack
{
    return [TableUtils getObject:d ofClass:[NSString class] forKey:key fallback:fallBack];
}

+ (NSArray*)getArray:(NSDictionary*)d forKey:(NSString*)key fallback:(NSArray*)fallBack
{
    return [TableUtils getObject:d ofClass:[NSArray class] forKey:key fallback:fallBack];
}

+ (SEL)getSelector:(NSDictionary*)d forKey:(NSString*)key fallback:(SEL)fallBack
{
    NSValue *obj=[d objectForKey:key];
    if (obj!=nil && [obj isKindOfClass:[NSValue class]])
    {
        SEL sel = [obj pointerValue];
        return sel;
    }
    else if (obj!=nil && [obj isKindOfClass:[NSString class]])
    {
        NSString *str = (NSString*)obj;
        SEL sel = NSSelectorFromString(str);
        return sel;
    }
    return fallBack;
}

@end

@implementation TableAdapterRowConfig

- (id)init
{
    self = [super init];
    if (self)
    {
        [self setupWithDictionary:nil];
    }
    return self;
}

- (id)initWithDictionary:(NSDictionary*)d
{
    self = [super init];
    if (self)
    {
        [self setupWithDictionary:d];
    }
    return self;
}

- (void)setupWithDictionary:(NSDictionary*)d
{
    self.editable = [[TableUtils getNumber:d forKey:@"editable" fallback:@(YES)] boolValue];
    self.rowType = [[TableUtils getNumber:d forKey:@"rowType" fallback:@(TableRowTypeUnknown)] integerValue];
    self.simpleCheckbox = [[TableUtils getNumber:d forKey:@"simpleCheckbox" fallback:@(NO)] boolValue];
    self.inlineTextEditing = [[TableUtils getNumber:d forKey:@"inlineTextEditing" fallback:@(NO)] boolValue];
    self.secureTextEditing = [[TableUtils getNumber:d forKey:@"secureTextEditing" fallback:@(NO)] boolValue];
    self.keyboardType = [[TableUtils getNumber:d forKey:@"keyboardType" fallback:@(KeyboardTypeIgnore)] integerValue];
    self.correctionType = [[TableUtils getNumber:d forKey:@"correctionType" fallback:@(CorrectionTypeIgnore)] integerValue];
    self.capitalizationType = [[TableUtils getNumber:d forKey:@"capitalizationType" fallback:@(CapitalizationTypeIgnore)] integerValue];
    self.displayName = [TableUtils getString:d forKey:@"displayName" fallback:nil];
    self.dateFormat = [TableUtils getString:d forKey:@"dateFormat" fallback:nil];
    self.timeFormat = [TableUtils getString:d forKey:@"timeFormat" fallback:nil];
    self.dateTimeFormat = [TableUtils getString:d forKey:@"dateTimeFormat" fallback:nil];
    self.singleChoiceOptions = [TableUtils getArray:d forKey:@"singleChoiceOptions" fallback:nil];
    self.clicked = [TableUtils getSelector:d forKey:@"clicked" fallback:nil];
}

@end

@implementation TableAdapterRowConfigs
@synthesize configs;

- (id)init
{
    self = [super init];
    if (self)
    {
        self.configs = [[NSMutableDictionary alloc] init];
    }
    return self;
}

- (void)add:(NSString*)key config:(NSDictionary*)config
{
    TableAdapterRowConfig *conf = [[TableAdapterRowConfig alloc] initWithDictionary:config];
    [self.configs setObject:conf forKey:key];
}

- (TableAdapterRowConfig*)configForRow:(NSString*)rowName
{
    return self.configs[rowName];
}

@end

@implementation TextHelper

+ (NSString*)scrambledText:(NSString*)str
{
    if (str == nil || str.length==0)
        return @"";
    return [@"" stringByPaddingToLength:str.length withString:@"*" startingAtIndex:0];
}

+ (CGSize)stringSize:(NSString*)text font:(UIFont*)font constraint:(CGSize)constraint lineBreakMode:(NSLineBreakMode)lineBreakMode
{
    CGSize size;
    if ([text respondsToSelector:@selector(sizeWithAttributes:)])
    {
        NSDictionary *attributes = @{NSFontAttributeName:font};
        
        CGSize boundingBox = [text boundingRectWithSize:constraint options: NSStringDrawingUsesLineFragmentOrigin attributes:attributes context:nil].size;
        
        size = CGSizeMake(ceil(boundingBox.width), ceil(boundingBox.height));
    }
    else
    {
        #pragma GCC diagnostic push
        #pragma GCC diagnostic ignored "-Wdeprecated-declarations"
        size = [text sizeWithFont:font constrainedToSize:constraint lineBreakMode:lineBreakMode];
        #pragma GCC diagnostic pop
    }
    
    return size;
}

+ (CGSize)stringSize:(NSString*)text font:(UIFont*)font
{
    if ([text respondsToSelector:@selector(sizeWithAttributes:)])
    {
        NSDictionary* attribs = @{NSFontAttributeName:font};
        return ([text sizeWithAttributes:attribs]);
    }
    #pragma GCC diagnostic push
    #pragma GCC diagnostic ignored "-Wdeprecated-declarations"
    return ([text sizeWithFont:font]);
    #pragma GCC diagnostic pop
}

@end
