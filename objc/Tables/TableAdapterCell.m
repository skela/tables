//
//  TableAdapterCell.m
//  tables
//
//  Created by Aleksander Slater on 08/09/2014.
//  Copyright (c) 2014 Davincium. All rights reserved.
//

#import "TableAdapterCell.h"

@implementation TableAdapterCell

- (id)initWithStyle:(UITableViewCellStyle)style reuseIdentifier:(NSString *)reuseIdentifier
{
    self = [super initWithStyle:style reuseIdentifier:reuseIdentifier];
    if (self) {
        // Initialization code
    }
    return self;
}

- (void)layoutSubviews
{
    [super layoutSubviews];
    
    if ([self.accessoryView isKindOfClass:[UITextField class]])
    {
        [self.textLabel sizeToFit];
        
        CGFloat w = self.bounds.size.width;
        CGFloat x = self.textLabel.frame.origin.x + self.textLabel.frame.size.width + 10;
        self.accessoryView.frame = CGRectMake (x, 0, w - x - 10, 44);
    }
}

@end
