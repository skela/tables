//
//  TableSource.m
//  tables
//
//  Created by Aleksander Slater on 07/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import "TableSource.h"
#import "TablePropertyScanner.h"

@interface TableSource()
@property(nonatomic,strong) NSMutableDictionary*formatters;
@property(nonatomic,strong) NSMutableArray*properties;
@end

@implementation TableSource

- (id)initWithData:(NSObject*)data defaultStringRowType:(TableRowType)rowType
{
    self = [super init];
    if (self)
    {
        self.formatters = [[NSMutableDictionary alloc] init];
        self.properties = [[NSMutableArray alloc] init];
        self.data = data;
        self.defaultStringRowType= rowType;
        
        [self reloadProperties];
    }
    return self;
}

- (id)initWithData:(NSObject*)data
{
    self = [self initWithData:data defaultStringRowType:TableRowTypeText];
    if (self)
    {
        
    }
    return self;
}

- (void)reloadProperties
{
    [self.properties removeAllObjects];
    NSArray *objectProperties = [TablePropertyScanner propertiesForObject:self.data];
    for (NSDictionary*d in objectProperties)
    {
        [self.properties addObject:[d objectForKey:TablePropertyScannerKey]];
    }
}

- (NSDateFormatter*)formatterForFormat:(NSString*)format
{
    NSDateFormatter *f = [self.formatters objectForKey:format];
    if (f==nil)
    {
        f = [[NSDateFormatter alloc] init];
        [f setDateFormat:format];
        [self.formatters setObject:f forKey:format];
    }
    return f;
}

- (NSInteger)numberOfSections
{
    return 1;
}

- (NSInteger)rowsInSection:(NSInteger)section
{
    if (self.data != nil)
    {
        return self.properties.count;
    }
    return 0;
}

- (NSObject*)getValueAtRow:(NSInteger)row andSection:(NSInteger)section
{
    NSString *key = self.properties[row];
    NSObject *obj = [self.data valueForKey:key];
    return obj;
}

- (NSString*)getValueStringAtRow:(NSInteger)row andSection:(NSInteger)section
{
    NSObject *value = [self getValueAtRow:row andSection:section];
    NSString *text  = @"";
    if ([value isKindOfClass:[NSString class]])
        text = (NSString*)value;
    return text;
}

- (NSNumber*)getValueNumberAtRow:(NSInteger)row andSection:(NSInteger)section
{
    NSObject *value = [self getValueAtRow:row andSection:section];
    if ([value isKindOfClass:[NSNumber class]])
        return (NSNumber*)value;
    return nil;
}

- (NSDate*)getValueDateAtRow:(NSInteger)row andSection:(NSInteger)section
{
    NSObject *value = [self getValueAtRow:row andSection:section];
    if ([value isKindOfClass:[NSDate class]])
        return (NSDate*)value;
    return nil;
}

- (BOOL)getValueBoolAtRow:(NSInteger)row andSection:(NSInteger)section
{
    NSNumber *number = [self getValueNumberAtRow:row andSection:section];
    return [number boolValue];
}

- (void)setValue:(NSObject*)obj atRow:(NSInteger)row andSection:(NSInteger)section
{
    NSString *key = self.properties[row];
    [self.data setValue:obj forKey:key];
}

- (NSString*)displayName:(id<ITableAdapterRowConfigurator>)configurator row:(NSInteger)row section:(NSInteger)section
{
    NSString *key = self.properties[row];
    TableAdapterRowConfig*config = [configurator configForRow:key];
    if (config!=nil && config.displayName!=nil)
        return config.displayName;
    return key;
}

- (NSString*)displayDate:(id<ITableAdapterRowConfigurator>)configurator row:(NSInteger)row section:(NSInteger)section date:(NSDate*)date rowType:(TableRowType)rowType
{
    NSString *propertyName = [self rowName:row andSection:section];
    TableAdapterRowConfig *s = [self rowSetting:configurator propertyName:propertyName];
    NSString *format = nil;
    switch (rowType)
    {
        case TableRowTypeDate:
            format = @"d MMM yyyy";
            break;
        case TableRowTypeTime:
            format = @"HH:mm";
            break;
        case TableRowTypeDateTime:
            format = @"HH:mm d MMM yyyy";
            break;
        default: break;
    }
    if (s != nil)
    {
        switch (rowType)
        {
            case TableRowTypeDate:
                if (s.dateFormat!=nil)
                    format = s.dateFormat;
                break;
            case TableRowTypeTime:
                if (s.timeFormat!=nil)
                    format = s.timeFormat;
                break;
            case TableRowTypeDateTime:
                if (s.dateTimeFormat!=nil)
                    format = s.dateTimeFormat;
                break;
            default: break;
        }
    }
    return format == nil ? [date description] : [[self formatterForFormat:format] stringFromDate:date];
}

- (NSString*)rowName:(NSInteger)row andSection:(NSInteger)section
{
    NSString *propertyName = self.properties[row];
    return propertyName;
}

- (TableAdapterRowConfig*)rowSetting:(id<ITableAdapterRowConfigurator>)configurator propertyName:(NSString*)propertyName
{
    TableAdapterRowConfig *settings = nil;
    if ([self.data respondsToSelector:@selector(configForRow:)])
    {
        id<ITableAdapterRowConfigurator>temp = (id<ITableAdapterRowConfigurator>)self.data;
        settings = [temp configForRow:propertyName];
    }
    else if (configurator != nil)
        settings = [configurator configForRow:propertyName];
    return settings;
}

- (TableRowType)rowType:(id<ITableAdapterRowConfigurator>)configurator row:(NSInteger)row section:(NSInteger)section
{
    NSString *propertyName = [self rowName:row andSection:section];
    NSObject *prop = [self getValueAtRow:row andSection:section];
    TableAdapterRowConfig*s = [self rowSetting:configurator propertyName:propertyName];
    if (s != nil && s.rowType != TableRowTypeUnknown)
        return s.rowType;
    TableRowType rowType = self.defaultStringRowType;
    if ([prop isKindOfClass:[NSString class]])
        rowType = self.defaultStringRowType;
    else if ([prop isKindOfClass:[NSNumber class]])
        rowType = TableRowTypeCheckbox;
    else if ([prop isKindOfClass:[NSDate class]])
        rowType = TableRowTypeDateTime;
    return rowType;
}

- (BOOL)rowEditable:(id<ITableAdapterRowConfigurator>)configurator row:(NSInteger)row section:(NSInteger)section
{
    //NSObject *prop = nil; // TODO: Get Property
    NSString *propertyName = [self rowName:row andSection:section];
    //var editable = prop.CanWrite && prop.SetMethod.IsPublic;
    BOOL editable = YES;
    TableAdapterRowConfig *settings = [self rowSetting:configurator propertyName:propertyName];
    if (settings!=nil)
        editable = settings.editable;
    return editable;
}

@end
