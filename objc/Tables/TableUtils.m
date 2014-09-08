//
//  TableUtils.m
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import "TableUtils.h"

@implementation TableAdapterRowConfig

- (id)init
{
    self = [super init];
    if (self)
    {
        self.editable = YES;
        self.rowType = TableRowTypeUnknown;
        self.simpleCheckbox = NO;
        self.inlineTextEditing = NO;
        self.secureTextEditing = NO;
        self.keyboardType = KeyboardTypeIgnore;
        self.correctionType = CorrectionTypeIgnore;
        self.capitalizationType = CapitalizationTypeIgnore;
    }
    return self;
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

@end

@implementation TableHelper

+ (CGSize)stringSize:(NSString*)text font:(UIFont*)font constraint:(CGSize)constraint lineBreakMode:(UILineBreakMode)lineBreakMode
{
    
}

+ (CGSize)stringSize:(NSString*)text font:(UIFont*)font
{
    
}

@end